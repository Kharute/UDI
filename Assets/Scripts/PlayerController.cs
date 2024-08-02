using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    NavMeshAgent agent;

    GameObject goal;

    private void Awake()
    {
        goal = GameObject.FindWithTag("Goal");
        agent = GetComponent<NavMeshAgent>();

        if(goal != null)
        {
            //agent.transform.position = goal.transform.position;
            agent.destination = goal.transform.position;
        }
    }

    private void Update()
    {
        
    }

}
