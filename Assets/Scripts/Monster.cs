using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Monster : MonoBehaviour
{
    NavMeshAgent agent;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }
    void FixedUpdate()
    {
        if (agent != null)
        {
            var Player = GameObject.FindWithTag("Player");
            agent.SetDestination(Player.transform.position);
        }
    }
}
