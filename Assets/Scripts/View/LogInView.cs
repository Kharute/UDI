using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class LogInView : MonoBehaviour
{
    [SerializeField] Button loginButton;

    [SerializeField] TMP_InputField usernameField;
    [SerializeField] TMP_InputField passwordField;
    [SerializeField] TextMeshProUGUI feedbackText;

    private void Awake()
    {
        OnClick_MenuButton();
    }
    void Start()
    {    
        loginButton.onClick?.AddListener(OnClick_LoginButton);   
    }

    public void OnClick_MenuButton()
    {
        transform.GetChild(0).gameObject.SetActive(true);  // MainPanel
        transform.GetChild(1).gameObject.SetActive(false); // LoginPanel
    }

    public void OnClick_EnterButton()
    {
        transform.GetChild(0).gameObject.SetActive(false);  // MainPanel
        transform.GetChild(1).gameObject.SetActive(true); // LoginPanel
    }

    public void OnClick_LoginButton()
    {
        string username = usernameField.text;
        string password = passwordField.text;

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            feedbackText.text = "Please enter both username and password.";
            return;
        }
        else
        {
            feedbackText.text = DataBaseManager.Inst.RequestLogin(username, password);
            //[TODO]없다면 호출하면 안됨
            gameObject.SetActive(false); // 상황에 맞게 조절해야 함.
        }
    }
}

