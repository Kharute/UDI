using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Monster : MonoBehaviour
{
    #region Fields
    // MonsterPlant만의 게임 오브젝트(Projectile)
    [SerializeField] private GameObject projectile;
    [SerializeField] private float shootSpeed = 800.0f;
    [SerializeField] private Transform spawnPosition;
    [SerializeField] private ParticleSystem stabAttack;

    private float followDistance = 50f;
    #endregion
    NavMeshAgent agent;
    Action action;

    Transform player_transform;
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        if (agent != null)
        {
            var Player = GameObject.FindWithTag("Player");
            agent.SetDestination(Player.transform.position);
            player_transform = Player.transform;
            //action += TracePlayer();
        }   
    }
    private void Update()
    {
        float distance = Vector3.Distance(player_transform.position, transform.position);

        if (distance < followDistance)
        {
            action?.Invoke();
        }
    }

    void TracePlayer()
    {

    }
}
