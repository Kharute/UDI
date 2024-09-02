using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum State
{
    Idle,
    Run,
    Attack
}


public class PlayerController : MonoBehaviour
{
    public List<GameObject> AllObjects;

    [SerializeField]
    float chaseDistance = 30.0f;
    float attackDistance = 2.0f;

    public NavMeshAgent _agent;
    public Animator _anim;
    public Rigidbody _rig;

    [SerializeField]
    private GameObject target;
    private GameObject goal;

    public SphereCollider _col;

    public State state;
    private FSM fsm;

    private void Awake()
    {
        goal = GameObject.FindWithTag("Goal");
        _agent = GetComponent<NavMeshAgent>();
        _anim = GetComponent<Animator>();
        _rig = GetComponent<Rigidbody>();

        if (goal != null)
        {
            _agent.destination = goal.transform.position;
        }
    }
    private void Start()
    {
        fsm = new FSM();
        fsm.ChangeState(new PlayerIdleState(this, _agent, _anim, _rig));
        state = State.Idle;
    }

    // 로직은 AllEnemy가 1 이상일 때 적을 추적,
    // 0이하면 1초마다 Find

    //기능 갈라서 상황 별로 돌아가도록 설계
    /*
     1. 앞으로 진행.
     2. 적 시야내에 들었을 경우.
     3. 적과 근접했을 경우.
     4. 적을 죽였을 경우
     5. 적을 전부 죽였을 경우.
     6. 리셋됐을 경우.

    */
    private void FixedUpdate()
    {
        

        fsm.Update();

        // 기능 케이스

        if (_agent != null)
        {
            if (AllObjects.Count <= 0)
            {
                LoadTarget();
            }
            else
            {
                if (target == null)
                {
                    foreach (GameObject go in AllObjects)
                    {
                        if(go.activeInHierarchy)
                        {
                            target = go;
                            StartCoroutine("ChaseMonster");
                            break;
                        }
                    }
                    if (target == null)
                    {
                        target = goal;
                        StartCoroutine("ChaseMonster");
                        fsm.ChangeState(new PlayerChaseState(this, _agent, _anim, _rig));
                    }
                }
                else if (target != null)
                {
                    float dist = Vector3.Distance(gameObject.transform.position, target.transform.position);
                    State tempState = State.Idle;

                    if (dist < attackDistance)
                    {
                        tempState = State.Attack;
                    }

                    else if (dist < chaseDistance)
                    {
                        tempState = State.Run;
                    }

                    if (state != tempState)
                    {
                        switch (tempState)
                        {
                            case State.Attack:
                                fsm.ChangeState(new PlayerAttackState(this, _agent, _anim, _rig));
                                //state = State.Attack;
                                break;
                            case State.Run:
                                StartCoroutine("ChaseMonster");
                                fsm.ChangeState(new PlayerChaseState(this, _agent, _anim, _rig));
                                //state = State.Run;
                                break;
                        }
                    }
                }
            }
        }
    }

    IEnumerator ChaseMonster()
    {
        if (target != null)
        {
            while (target.activeInHierarchy)
            {
                _agent.destination = target.transform.position;
                yield return new WaitForSeconds(0.5f);
            }
            target = null;
            state = State.Run;
        }
    }
    public void Attack()
    {
        _col.gameObject.SetActive(true);
    }

    protected void LoadTarget()
    {
        AllObjects.AddRange(GameObject.FindGameObjectsWithTag("Monster"));
    }

}
