using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    [SerializeField] GameObject monster_prefab;
    [SerializeField] Transform[] spawnPoint;

    List<GameObject> monster_target = new List<GameObject>();

    private void Awake()
    {
        if (spawnPoint.Length > 0)
        {
            for (int i = 0; i < spawnPoint.Length; i++)
            {
                var monster = Instantiate(monster_prefab, spawnPoint[i]);
                monster.SetActive(true);
                monster_target.Add(monster);

            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            
        }
    }
}
