using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damage : MonoBehaviour
{
    float disTime;

    private void OnEnable()
    {
        StartCoroutine("SetOffObj");
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Monster")
        {
            gameObject.SetActive(false);
        }
    }

    IEnumerator SetOffObj()
    {
        disTime = 0.2f;
        while (disTime > 0)
        {
            disTime -= Time.deltaTime;
            yield return new WaitForSeconds(Time.deltaTime);
        }
        gameObject.SetActive(false);
    }
}
