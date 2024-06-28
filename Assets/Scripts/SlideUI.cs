using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlideUI : MonoBehaviour
{
    [SerializeField] bool isOpened = false;
    [SerializeField] float moveValue = 200f;
    [SerializeField] float rolltime = 1.5f;

    public void OnClick_UI()
    {
        StartCoroutine(SlideStateChange());
    }

    IEnumerator SlideStateChange()
    {
        isOpened = !isOpened;

        float upValue = isOpened ? 1 : -1;
        Vector3 vec = transform.position;
        transform.DOMove(vec + Vector3.up * moveValue * upValue, rolltime);

        yield return new WaitForSeconds(rolltime);
    }

}
