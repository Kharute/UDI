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

    // ������ AllEnemy�� 1 �̻��� �� ���� ����,
    // 0���ϸ� 1�ʸ��� Find

    //��� ���� ��Ȳ ���� ���ư����� ����
    /*
     1. ������ ����.
     2. �� �þ߳��� ����� ���.
     3. ���� �������� ���.
     4. ���� �׿��� ���
     5. ���� ���� �׿��� ���.
     6. ���µ��� ���.

    */
    private void FixedUpdate()
    {
        

        fsm.Update();

        // ��� ���̽�

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
