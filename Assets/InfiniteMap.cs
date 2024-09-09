using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfiniteMap : MonoBehaviour
{
    [SerializeField] Collider spawnCollider1;
    [SerializeField] Collider spawnCollider2;
    [SerializeField] Collider goalCollider;

    private void Update()
    {
        if(spawnCollider1.isTrigger)
        {
            OnTriggerEnter(spawnCollider1);
        }
        if (spawnCollider2.isTrigger)
        {
            OnTriggerEnter(spawnCollider2);
        }
        if (goalCollider.isTrigger)
        {
            OnTriggerEnter(goalCollider);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        switch(other.gameObject.name)
        {
            case "spawnCollider1":
                MonsterSpawner s1 = spawnCollider1.GetComponent<MonsterSpawner>();
                if (s1 != null)
                    s1.MonsterSpawn();
                /*PlayerController p = other.gameObject.GetComponent<PlayerController>();

                foreach (GameObject go in p.AllObjects)
                {
                    go.gameObject.SetActive(true);
                } */
                break;

            case "spawnCollider2":
                MonsterSpawner s2 = spawnCollider2.GetComponent<MonsterSpawner>();
                if (s2 != null)
                    s2.MonsterSpawn();
                break;

            case "goalCollider":
                if (other.CompareTag("Player"))
                    other.gameObject.transform.position = Vector3.zero;
                break;
        }
    }
}
