using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class PlayerStateBase
{
    protected PlayerController _player;
    protected NavMeshAgent _agent;
    protected Animator _anim;
    protected Rigidbody _rig;

    public PlayerStateBase(PlayerController player, NavMeshAgent agent, Animator anim, Rigidbody rig)
    {
        _player = player;
        _agent = agent;
        _anim = anim;
        _rig = rig;
    }
}

public class PlayerIdleState : PlayerStateBase, IState
{
    public PlayerIdleState(PlayerController player, NavMeshAgent agent, Animator anim, Rigidbody rig)
        : base(player, agent, anim, rig) { }

    public void Enter()
    {
        _anim.SetBool("Idle", true);
        _agent.isStopped = true;
    }

    public void Update()
    {
        // Check for nearby enemies or objectives
        if (_player.target != null)
        {
            float distanceToTarget = Vector3.Distance(_player.transform.position, _player.target.transform.position);
            if (distanceToTarget <= _player.chaseDistance)
            {
                _player.SwitchToState(State.Run);
            }
        }
    }

    public void Exit()
    {
        _anim.SetBool("Idle", false);
        _agent.isStopped = false;
    }
}

public class PlayerChaseState : PlayerStateBase, IState
{
    public PlayerChaseState(PlayerController player, NavMeshAgent agent, Animator anim, Rigidbody rig)
        : base(player, agent, anim, rig) { }

    public void Enter()
    {
        _anim.SetBool("Run", true);
        _agent.isStopped = false;
    }

    public void Update()
    {
        if (_player.target != null)
        {
            _agent.SetDestination(_player.target.transform.position);

            float distanceToTarget = Vector3.Distance(_player.transform.position, _player.target.transform.position);
            if (distanceToTarget <= _player.attackDistance)
            {
                _player.SwitchToState(State.Attack);
            }
            else if (distanceToTarget > _player.chaseDistance)
            {
                _player.SwitchToState(State.Idle);
            }
        }
        else
        {
            _player.SwitchToState(State.Idle);
        }

        // Update rotation to face movement direction
        if (_agent.velocity.sqrMagnitude > Mathf.Epsilon)
        {
            _player.transform.rotation = Quaternion.LookRotation(_agent.velocity.normalized);
        }
    }

    public void Exit()
    {
        _anim.SetBool("Run", false);
    }
}

public class PlayerAttackState : PlayerStateBase, IState
{
    private float attackCooldown = 1f;
    private float lastAttackTime;

    public PlayerAttackState(PlayerController player, NavMeshAgent agent, Animator anim, Rigidbody rig)
        : base(player, agent, anim, rig) { }

    public void Enter()
    {
        _anim.SetBool("Attack", true);
        _agent.isStopped = true;
        lastAttackTime = -attackCooldown; // Allow immediate first attack
    }

    public void Update()
    {
        if (_player.target != null)
        {
            // Face the target
            Vector3 directionToTarget = (_player.target.transform.position - _player.transform.position).normalized;
            _player.transform.rotation = Quaternion.LookRotation(new Vector3(directionToTarget.x, 0, directionToTarget.z));

            float distanceToTarget = Vector3.Distance(_player.transform.position, _player.target.transform.position);

            if (distanceToTarget <= _player.attackDistance)
            {
                if (Time.time - lastAttackTime >= attackCooldown)
                {
                    PerformAttack();
                    lastAttackTime = Time.time;
                }
            }
            else if (distanceToTarget > _player.attackDistance)
            {
                _player.SwitchToState(State.Run);
            }
        }
        else
        {
            _player.SwitchToState(State.Idle);
        }
    }

    private void PerformAttack()
    {
        _player.Attack();
    }

    public void Exit()
    {
        _anim.SetBool("Attack", false);
        _agent.isStopped = false;
    }
}


public class MonsterStateBase
{
    private Monster _monster;
    private NavMeshAgent _agent;
    private Animator _anim;
    private Rigidbody _rig;

    public Monster Monster { get { return _monster; } set { _monster = value; } }
    public NavMeshAgent Agent { get { return _agent; } set { _agent = value; } }
    public Animator Animator { get { return _anim; } set { _anim = value; } }
    public Rigidbody Rig { get { return _rig; } set { _rig = value; } }
}

public class MonsterIdleState : MonsterStateBase, IState
{
    public MonsterIdleState(Monster monster, NavMeshAgent agent, Animator anim, Rigidbody rig)
    {
        Monster = monster;
        Monster.state = State.Idle;
        Agent = agent;
        Animator = anim;
        Rig = rig;
    }

    public void Enter()
    {

    }

    public void Update()
    {
        Rig.constraints = RigidbodyConstraints.FreezeAll;
    }

    public void Exit()
    {
        Rig.constraints = RigidbodyConstraints.None;
    }
}

public class MonsterChaseState : MonsterStateBase, IState
{
    public MonsterChaseState(Monster monster, NavMeshAgent agent, Animator anim, Rigidbody rig)
    {
        Monster = monster;
        Monster.state = State.Idle;
        Agent = agent;
        Animator = anim;
        Rig = rig;
    }

    public void Enter()
    {

    }

    public void Update()
    {
        Vector3 lookrotation = Agent.steeringTarget - Monster.transform.position;
        Monster.transform.rotation = Quaternion.Slerp(Monster.transform.rotation, Quaternion.LookRotation(lookrotation), Time.deltaTime);

        if (Monster.tempX != 0 && Monster.tempY != 0)
        {
            float distanceX = Mathf.Abs(Monster.tempX - Monster.transform.position.x);
            float distanceY = Mathf.Abs(Monster.tempY - Monster.transform.position.z);

            Animator.SetFloat("FloatX", distanceX);
            Animator.SetFloat("FloatY", distanceY);
        }

        Monster.tempX = Monster.transform.position.x;
        Monster.tempY = Monster.transform.position.z;
    }

    public void Exit()
    {
        
    }
}

public class MonsterAttackState : MonsterStateBase, IState
{
    public MonsterAttackState(Monster monster, NavMeshAgent agent, Animator anim, Rigidbody rig)
    {
        Monster = monster;
        Monster.state = State.Idle;
        Agent = agent;
        Animator = anim;
        Rig = rig;
    }

    public void Enter()
    {
        Animator.SetBool("Attack", true);
        Rig.constraints = RigidbodyConstraints.FreezeAll;
    }

    public void Update()
    {
        // 대기 중 행동 로직
        // 예: 플레이어 감지 시 공격 상태로 전환
    }

    public void Exit()
    {
        Animator.SetBool("Attack", false);
        Rig.constraints = RigidbodyConstraints.None;
    }
}


