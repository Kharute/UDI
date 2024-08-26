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

    // ������ AllEnemy�� 1 �̻��� �� ���� ����,
    // 0���ϸ� 1�ʸ��� Find

    private void FixedUpdate()
    {
        fsm.Update();

        if (_agent != null)
        {
            //[TODO] �ð� ���� �ɾ� �� ��.
            if (AllObjects.Count <= 0)
            {
                LoadTarget();
            }
            else
            {
                //[TODO] �ϴ� ���� ���� ��쿡�� ȣ��

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
