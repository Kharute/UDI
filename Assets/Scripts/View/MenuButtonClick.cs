
using UnityEngine;
using DG.Tweening;
using System.Collections;


public class MenuButtonClick : MonoBehaviour
{
    [SerializeField]
    float moveValue = 300f;
    [SerializeField]
    GameObject[] GameObjects;
    [SerializeField]
    float rolltime = 1.0f;
    bool isMenuOpen = false;

    public void OnButtonClick_Menu()
    {
        StartCoroutine(MenuStateChange());
    }

    IEnumerator MenuStateChange()
    {
        isMenuOpen = !isMenuOpen;
        
        float upValue = 0;
        foreach (var go in GameObjects)
        {
            if (isMenuOpen)
            {
                go.SetActive(true);
            }
            upValue += isMenuOpen ? 1 : -1;
            Vector3 vec = go.transform.position;
            go.transform.DOMove(vec + Vector3.up * moveValue * upValue, rolltime);
        }
        yield return new WaitForSeconds(rolltime);

        foreach (var go in GameObjects)
        {
            if (!isMenuOpen)
            {
                go.SetActive(false);
            }
        }
    }
}
