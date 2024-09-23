using DG.Tweening.Core.Easing;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] GameObject inGameView;
    [SerializeField] GameObject outGameView;
    [SerializeField] GameObject player;

    private static UIManager _instance;
    public static UIManager Inst
    {
        get
        {
            if (_instance == null)
            {
                // ���� GameManager�� ���ٸ� ����
                GameObject singletonObject = new GameObject();
                _instance = singletonObject.AddComponent<UIManager>();
                singletonObject.name = typeof(UIManager).ToString() + " (Singleton)";
            }
            return _instance;
        }
    }

    private void Awake()
    {
        // �̹� �ν��Ͻ��� �����ϸ� �ߺ� �ı� ����
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(gameObject); // �ߺ��� �̱����� ������ �ʵ��� ���� ������ ���� �ı�
        }
    }

    public void GameStart()
    {
        outGameView.SetActive(false);
        inGameView.SetActive(true);
        player.gameObject.SetActive(true);
    }
}
