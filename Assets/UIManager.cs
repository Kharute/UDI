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
                // 씬에 GameManager가 없다면 생성
                GameObject singletonObject = new GameObject();
                _instance = singletonObject.AddComponent<UIManager>();
                singletonObject.name = typeof(UIManager).ToString() + " (Singleton)";
            }
            return _instance;
        }
    }

    private void Awake()
    {
        // 이미 인스턴스가 존재하면 중복 파괴 방지
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(gameObject); // 중복된 싱글턴이 생기지 않도록 새로 생성된 것을 파괴
        }
    }

    public void GameStart()
    {
        outGameView.SetActive(false);
        inGameView.SetActive(true);
        player.gameObject.SetActive(true);
    }
}
