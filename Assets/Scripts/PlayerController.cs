using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    Transform[] transforms;

    private void Update()
    {
        if(transforms.Length > 0)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                gameObject.transform.position = transforms[0].position;
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                gameObject.transform.position = transforms[1].position;
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                gameObject.transform.position = transforms[2].position;
            }
            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                gameObject.transform.position = transforms[3].position;
            }
            if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                gameObject.transform.position = transforms[4].position;

            }
        }
        
    }

}
