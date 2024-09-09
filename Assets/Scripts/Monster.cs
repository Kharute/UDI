using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class Monster : MonoBehaviour
{
    #region Fields

    [SerializeField] private GameObject projectile;
    [SerializeField] private float shootSpeed = 800.0f;
    [SerializeField] private Transform spawnPosition;
    [SerializeField] private ParticleSystem stabAttack;

    GameObject Player;
    NavMeshAgent _agent;
    Animator _anim;
    Rigidbody _rig;

    public State state;
    private FSM fsm;

    float chaseDistance = 20.0f;
    float attackDistance = 2.0f;

    public float tempX, tempY;

    public float MaxHP = 10f;
    private float _hp = 10f;

    public float HP
    {
        get { return _hp; }
        set { _hp = value; }
    }

    #endregion

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _anim = GetComponent<Animator>();
        _rig = GetComponent<Rigidbody>();
        _agent.destination = transform.position;
        gameObject.tag = "Monster";

    }

    private void Start()
    {
        fsm = new FSM();
        fsm.ChangeState(new MonsterIdleState(this, _agent, _anim, _rig));

        if (_agent != null)
        {
            Player = GameObject.FindWithTag("Player");
        }
    }
    void Update()
    {
        fsm.Update();
    }

    private void FixedUpdate()
    {
        if (_agent != null)
        {
            State tempState = State.Idle;
            float dist = Vector3.Distance(gameObject.transform.position, Player.transform.position);

            if (dist < attackDistance)
                tempState = State.Attack;
            
            else if (dist < chaseDistance)
                tempState = State.Run;
            
            if (state != tempState)
            {
                switch (tempState)
                {
                    case State.Attack:
                        fsm.ChangeState(new MonsterAttackState(this, _agent, _anim, _rig));
                        //state = State.Attack;
                        break;
                    case State.Run:
                        StartCoroutine("TracePlayer");
                        fsm.ChangeState(new MonsterChaseState(this, _agent, _anim, _rig));
                        //state = State.Run;
                        break;
                }
            }
        }
    }

    IEnumerator TracePlayer()
    {
        while (Player.activeInHierarchy)
        {
            _agent.destination = Player.transform.position;
            yield return new WaitForSeconds(0.5f);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        HP -= 1;

        if(HP <= 0)
        {
            gameObject.transform.localPosition = Vector3.zero;
            gameObject.SetActive(false);
            HP = MaxHP;
        }
    }
}
