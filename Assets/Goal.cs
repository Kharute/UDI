using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            other.gameObject.transform.position = Vector3.zero;
            PlayerController p = other.gameObject.GetComponent<PlayerController>();

            foreach (GameObject go in p.AllObjects)
            {
                go.gameObject.SetActive(true);
            }
        }
    }
}
