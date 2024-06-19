using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlideUI : MonoBehaviour
{
    [SerializeField]
    Transform[] _trs;

    [SerializeField]
    bool isOpened;

    [SerializeField]
    float moveSpeed = 3f;

    Action SetUIAction;

    private void Awake()
    {
        //var obj = GameObject.Child<SlideUI>();    }
    }
    private void Update()
    {
        ChangePosition();
    }
    public void OnClick_UI()
    {
        isOpened = !isOpened;
    }

    private void ChangePosition()
    {
        if(_trs != null)
        {
            var pos = isOpened ? _trs[0].position : _trs[1].position;
            gameObject.transform.position = Vector2.Lerp(gameObject.transform.position, pos, moveSpeed * Time.deltaTime);
        }
    }

}
