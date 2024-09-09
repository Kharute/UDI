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

    public float chaseDistance = 30.0f;
    public float attackDistance = 2.0f;

    public NavMeshAgent _agent;
    public Animator _anim;
    public Rigidbody _rig;

    [SerializeField]
    public GameObject target;
    private GameObject goal;

    public SphereCollider _col;

    public State state;
    private FSM fsm;

    [SerializeField]
    private float targetUpdateInterval = 1f;
    private float lastTargetUpdateTime;

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
        lastTargetUpdateTime = Time.time;
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
        UpdateTarget();
        UpdateFSM();
    }

    private void UpdateTarget()
    {
        if (Time.time - lastTargetUpdateTime >= targetUpdateInterval)
        {
            lastTargetUpdateTime = Time.time;

            if (AllObjects.Count <= 0 || target == null || !target.activeInHierarchy)
            {
                LoadTarget();
            }

            GameObject closestTarget = FindClosestActiveTarget();
            if (closestTarget != null)
            {
                target = closestTarget;
                StartCoroutine(ChaseMonster());
            }
            else if (target == null || !target.activeInHierarchy)
            {
                target = goal;
                StartCoroutine(ChaseMonster());
            }
        }
    }

    private void UpdateFSM()
    {
        fsm.Update();

        if (target != null)
        {
            float dist = Vector3.Distance(transform.position, target.transform.position);
            State newState = DetermineState(dist);

            if (state != newState)
            {
                SwitchToState(newState);
            }
        }
    }

    private State DetermineState(float distance)
    {
        if (distance < attackDistance)
            return State.Attack;
        else if (distance < chaseDistance)
            return State.Run;
        else
            return State.Idle;
    }

    public void SwitchToState(State newState)
    {
        switch (newState)
        {
            case State.Attack:
                fsm.ChangeState(new PlayerAttackState(this, _agent, _anim, _rig));
                break;
            case State.Run:
                fsm.ChangeState(new PlayerChaseState(this, _agent, _anim, _rig));
                break;
            case State.Idle:
                fsm.ChangeState(new PlayerIdleState(this, _agent, _anim, _rig));
                break;
        }
        state = newState;
    }

    private GameObject FindClosestActiveTarget()
    {
        GameObject closest = null;
        float minDistance = float.MaxValue;
        foreach (GameObject go in AllObjects)
        {
            if (go.activeInHierarchy)
            {
                float distance = Vector3.Distance(transform.position, go.transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closest = go;
                }
            }
        }
        return closest;
    }

    IEnumerator ChaseMonster()
    {
        while (target != null && target.activeInHierarchy)
        {
            _agent.SetDestination(target.transform.position);
            yield return new WaitForSeconds(0.5f);
        }
    }

    public void Attack()
    {
        _col.gameObject.SetActive(true);
    }

    void LoadTarget()
    {
        AllObjects.Clear();
        AllObjects.AddRange(GameObject.FindGameObjectsWithTag("Monster"));
    }

}
