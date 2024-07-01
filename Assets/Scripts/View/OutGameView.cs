using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class OutGameView : MonoBehaviour
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
            DataBaseManager.Inst.RequestLogin(username, password);
            /*
             * LoginResponse log_res = 
             * 
                if (log_res.success)
                {
                        gameObject.SetActive(false);
                }
                else
                {
                        feedbackText.text = log_res.message;
                }*/
        }
    }

    public void OnDisable_Login()
    {
        gameObject.SetActive(false);
    }
    //[todo]
    IEnumerator Login_waitting()
    {
        while (true)
        {
            /*if (log_res.message == null)
            {
                break;
            }*/
            yield return new WaitForSeconds(0.1f);
        }
    }
}
