using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Monster : MonoBehaviour
{

    void Update()
    {
        fsm.Update();
    }

    #region Fields
    // MonsterPlant만의 게임 오브젝트(Projectile)
    [SerializeField] private GameObject projectile;
    [SerializeField] private float shootSpeed = 800.0f;
    [SerializeField] private Transform spawnPosition;
    [SerializeField] private ParticleSystem stabAttack;

    private float followDistance = 20f;
    #endregion

    NavMeshAgent agent;
    Action action;
    Transform player_transform;

    GameObject Player;

    private FSM fsm;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.stoppingDistance = 1f;
        agent.destination = this.transform.position;
        gameObject.tag = "Monster";
    }

    private void Start()
    {
        fsm = new FSM();
        fsm.ChangeState(new MonsterIdleState(this));

        if (agent != null)
        {
            Player = GameObject.FindWithTag("Player");
            action += TracePlayer;
        }   
    }
    private void FixedUpdate()
    {
        if (agent != null)
        {
            player_transform = Player.transform;

            float distance = Vector3.Distance(player_transform.position, transform.position);

            if (distance < followDistance)
            {
                TracePlayer();
                action?.Invoke();
            }
        }
    }

    void TracePlayer()
    {
        agent.SetDestination(Player.transform.position);
    }
}
