using System;
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
    float attackDistance = 5.0f;

    public NavMeshAgent _agent;
    public Animator _anim;
    public Rigidbody _rig;
    GameObject goal;

    public State state;
    private FSM fsm;

    private void Awake()
    {
        goal = GameObject.FindWithTag("Goal");
        _agent = GetComponent<NavMeshAgent>();
        _anim = GetComponent<Animator>();
        _rig = GetComponent<Rigidbody>();
        _agent.stoppingDistance = 1f;

        if (goal != null)
        {
            //agent.transform.position = goal.transform.position;
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

    private void FixedUpdate()
    {
        fsm.Update();

        if (_agent != null)
        {
            //[TODO] 시간 제한 걸어 둘 것.
            if (AllObjects.Count <= 0)
            {
                LoadTarget();
            }
            else
            {
                //[TODO] 하는 짓이 없는 경우에만 호출

                float dist = Vector3.Distance(gameObject.transform.position, AllObjects[0].transform.position);
                if (dist < attackDistance)
                {
                    fsm.ChangeState(new PlayerAttackState(this, _agent, _anim, _rig));
                }
                else if (dist < chaseDistance)
                {
                    _agent.destination = AllObjects[0].transform.position;
                    fsm.ChangeState(new PlayerChaseState(this, _agent, _anim, _rig));
                }
            }
        }
    }

    protected void LoadTarget()
    {
        AllObjects.AddRange(GameObject.FindGameObjectsWithTag("Monster"));
    }

}
