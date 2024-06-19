
using UnityEngine;
using DG.Tweening;


public class MenuButtonClick : MonoBehaviour
{
    [SerializeField]
    float moveValue = 300f;
    [SerializeField]
    GameObject[] GameObjects;
    bool isMenuOpen = false;

    public void OnButtonClick_Menu()
    {
        isMenuOpen = !isMenuOpen;

        float upValue = 0;
        foreach (var go in GameObjects)
        {
            //gameObject.SetActive(isMenuOpen);
            upValue += isMenuOpen ? 1 : -1;
            Vector3 vec  = go.transform.position;
            go.transform.DOMove(vec + Vector3.up * moveValue * upValue, 1f);
        }
    }
}
