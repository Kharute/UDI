using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;


public class PlayerStateBase
{
    private PlayerController _player;
    private NavMeshAgent _agent;
    private Animator _anim;
    private Rigidbody _rig;

    public PlayerController Player { get { return _player; } set {  _player = value; } }
    public NavMeshAgent Agent { get { return _agent; } set { _agent = value; } }
    public Animator Animator { get { return _anim; } set { _anim = value; } }
    public Rigidbody Rig { get { return _rig; } set { _rig = value; } }
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

public class PlayerIdleState : PlayerStateBase, IState
{
    public PlayerIdleState(PlayerController player, NavMeshAgent agent, Animator anim, Rigidbody rig)
    {
        Player = player;
        Player.state = State.Idle;
        Agent = agent;
        Animator = anim;
        Rig = rig;
    }

    public void Enter()
    {
        Animator.SetBool("Idle", true);
    }

    public void Update()
    {
        // 대기 중 행동 로직
        // 예: 자동 회복, 주변 아이템 수집 등
    }

    public void Exit()
    {
        Animator.SetBool("Idle", false);
    }
}

public class PlayerChaseState : PlayerStateBase, IState
{
    public PlayerChaseState(PlayerController player, NavMeshAgent agent, Animator anim, Rigidbody rig)
    {
        Player = player;
        Player.state = State.Run;
        Agent = agent;
        Animator = anim;
        Rig = rig;
    }

    public void Enter()
    {
        Animator.SetBool("Run", true);
    }

    public void Update()
    {
        Vector3 lookrotation = Player._agent.steeringTarget - Player.transform.position;
        Player.transform.rotation = Quaternion.Slerp(Player.transform.rotation, Quaternion.LookRotation(lookrotation), Time.deltaTime);
    }

    public void Exit()
    {
        Animator.SetBool("Run", false);
    }
}

public class PlayerAttackState : PlayerStateBase, IState
{
    public PlayerAttackState(PlayerController player, NavMeshAgent agent, Animator anim, Rigidbody rig)
    {
        Player = player;
        Player.state = State.Attack;
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
        // 예: 자동 회복, 주변 아이템 수집 등
    }

    public void Exit()
    {
        Animator.SetBool("Attack", false);
        Rig.constraints = RigidbodyConstraints.None;
    }
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


