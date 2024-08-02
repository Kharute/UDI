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
            //½ºÆùÇÏ¼î
        }
        if (spawnCollider2.isTrigger)
        {
            //½ºÆùÇÏ¼î
        }
    }
    void Awake()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {

        }
    }
}
