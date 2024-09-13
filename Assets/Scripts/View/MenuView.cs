
using UnityEngine;
using DG.Tweening;
using System.Collections;
using UnityEngine.UI;


public class MenuView : MonoBehaviour
{
    [SerializeField] Button[] menuButtons;
    [SerializeField] Button diveButton;

    [SerializeField] float moveValue = 40f;
    [SerializeField] float rolltime = 1.0f;
    
    bool isMenuOpen = false;

    public void OnButtonClick_Menu()
    {
        StartCoroutine(MenuStateChange());
    }

    public void OnButtonClick_Dive()
    {
        
    }


    IEnumerator MenuStateChange()
    {
        isMenuOpen = !isMenuOpen;
        
        float upValue = 0;
        foreach (var go in menuButtons)
        {
            if (isMenuOpen)
            {
                go.gameObject.SetActive(true);
            }
            upValue += isMenuOpen ? 1 : -1;
            Vector3 vec = go.transform.position;
            go.transform.DOMove(vec + Vector3.up * moveValue * upValue, rolltime);
        }
        yield return new WaitForSeconds(rolltime);

        foreach (var go in menuButtons)
        {
            if (!isMenuOpen)
            {
                go.gameObject.SetActive(false);
            }
        }
    }
}
