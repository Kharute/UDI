using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class PlayerIdleState : IState
{
    private PlayerController _player;
    private NavMeshAgent _agent;
    private Animator _anim;
    private Rigidbody _rig;

    public PlayerIdleState(PlayerController player, NavMeshAgent agent, Animator anim, Rigidbody rig)
    {
        this._player = player;
        _player.state = State.Idle;
        this._agent = agent;
        this._anim = anim;
        this._rig = rig;
    }

    public void Enter()
    {
        _player._anim.SetBool("Idle", true);
    }

    public void Update()
    {
        // 대기 중 행동 로직
        // 예: 자동 회복, 주변 아이템 수집 등
    }

    public void Exit()
    {
        _player._anim.SetBool("Idle", false);
    }
}

public class PlayerChaseState : IState
{
    private PlayerController _player;
    private NavMeshAgent _agent;
    private Animator _anim;
    private Rigidbody _rig;

    public PlayerChaseState(PlayerController player, NavMeshAgent agent, Animator anim, Rigidbody rig)
    {
        this._player = player;
        _player.state = State.Run;
        this._agent = agent;
        this._anim = anim;
        this._rig = rig;
    }

    public void Enter()
    {
        _player._anim.SetBool("Run", true);
    }

    public void Update()
    {
        // 대기 중 행동 로직
        // 예: 자동 회복, 주변 아이템 수집 등
    }

    public void Exit()
    {
        _player._anim.SetBool("Run", false);
    }
}

public class PlayerAttackState : IState
{
    private PlayerController _player;
    private NavMeshAgent _agent;
    private Animator _anim;
    private Rigidbody _rig;

    public PlayerAttackState(PlayerController player, NavMeshAgent agent, Animator anim, Rigidbody rig)
    {
        this._player = player;
        _player.state = State.Attack;
        this._agent = agent;
        this._anim = anim;
        this._rig = rig;
    }

    public void Enter()
    {
        _player._anim.SetBool("Attack", true);
        _rig.constraints = RigidbodyConstraints.FreezeRotationY;
    }

    public void Update()
    {
        
        // 대기 중 행동 로직
        // 예: 자동 회복, 주변 아이템 수집 등
    }

    public void Exit()
    {
        _player._anim.SetBool("Attack", false);
        _rig.constraints = RigidbodyConstraints.None;
    }
}


public class MonsterIdleState : IState
{
    private Monster monster;

    public MonsterIdleState(Monster monster)
    {
        this.monster = monster;
    }

    public void Enter()
    {
        // 대기 상태 진입 로직
    }

    public void Update()
    {
        // 대기 중 행동 로직
        // 예: 플레이어 감지 시 공격 상태로 전환
    }

    public void Exit()
    {
        // 대기 상태 종료 로직
    }
}


