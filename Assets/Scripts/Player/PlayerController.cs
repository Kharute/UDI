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
