using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlideUI : MonoBehaviour
{
    bool isMenuOpen = false;

    [SerializeField] bool isOpened;
    [SerializeField] float moveValue = 200f;
    [SerializeField] float rolltime = 1.5f;

    public void OnClick_UI()
    {
        StartCoroutine(SlideStateChange());
        isOpened = !isOpened;
    }

    IEnumerator SlideStateChange()
    {
        isMenuOpen = !isMenuOpen;

        float upValue = isMenuOpen ? 1 : -1;
        Vector3 vec = transform.position;
        transform.DOMove(vec + Vector3.up * moveValue * upValue, rolltime);
        
        yield return new WaitForSeconds(rolltime);
    }

}
