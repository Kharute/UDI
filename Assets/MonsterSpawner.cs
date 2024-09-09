using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    [SerializeField] GameObject[] monster_prefab;
    [SerializeField] Transform[] spawnPoint1;

    List<GameObject> monster_target = new List<GameObject>();

    int spawnTime = 0;
    private void Awake()
    {
        //여기서 풀땡기고
        if (spawnPoint1.Length > 0)
        {
            foreach (GameObject obj in monster_prefab)
            {
                for (int i = 0; i < spawnPoint1.Length; i++)
                {
                    var monster = Instantiate(obj, spawnPoint1[i]);
                    monster.SetActive(true);
                    monster_target.Add(monster);
                    monster.SetActive(false);
                }
            }
        }
    }

    public void MonsterSpawn()
    {
        for (int i = 0; i < spawnPoint1.Length; i++)
        {
            monster_target[i+spawnTime*spawnPoint1.Length].SetActive(false);
        }

        if(spawnTime == monster_prefab.Count())
            spawnTime = 0;
        else
            spawnTime++;
    }
}
