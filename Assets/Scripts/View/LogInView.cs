using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class LogInView : MonoBehaviour
{
    public TMP_InputField usernameField;
    public TMP_InputField passwordField;
    public Button loginButton;
    public TextMeshProUGUI feedbackText;

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

        StartCoroutine(Login(username, password));
    }
    IEnumerator Login(string username, string password)
    {
        string url = "http://localhost:3000/login";
        WWWForm form = new WWWForm();
        form.AddField("username", username);
        form.AddField("password", password);

        using (UnityWebRequest www = UnityWebRequest.Post(url, form))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                feedbackText.text = "Error: " + www.error;
            }
            else
            {
                ProcessLoginResponse(www.downloadHandler.text);
            }
        }
    }

    void ProcessLoginResponse(string response)
    {
        LoginResponse loginResponse = JsonUtility.FromJson<LoginResponse>(response);

        if (loginResponse.success)
        {
            feedbackText.text = "Login successful!";
            
            StartCoroutine(GetOrCreateUserDetails(loginResponse.userDetails.USER_ID));
            for (int i = 0; i < 4; i++)
            {
                transform.GetChild(i).gameObject.SetActive(false);
            }
        }
        else
        {
            feedbackText.text = "Login failed: " + loginResponse.message;
        }
    }

    IEnumerator GetOrCreateUserDetails(int userId)
    {
        string url = "http://localhost:3000/createUserDetails";
        WWWForm form = new WWWForm();
        form.AddField("userId", userId);

        using (UnityWebRequest www = UnityWebRequest.Post(url, form))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                feedbackText.text = "Error: " + www.error;
            }
            else
            {
                ProcessUserDetailsResponse(www.downloadHandler.text);
            }
        }
    }

    void ProcessUserDetailsResponse(string response)
    {
        UserDetailsResponse userDetailsResponse = JsonUtility.FromJson<UserDetailsResponse>(response);

        if (userDetailsResponse.success)
        {
            feedbackText.text = "User details retrieved successfully!";
            // 여기서 userDetailsResponse.userDetails를 사용하여 유저 정보를 처리
        }
        else
        {
            feedbackText.text = "Failed to retrieve user details: " + userDetailsResponse.message;
        }
    }
}

[System.Serializable]
public class LoginResponse
{
    public bool success;
    public string message;
    public UserDetails userDetails;
}

[System.Serializable]
public class UserDetailsResponse
{
    public bool success;
    public string message;
    public UserDetails userDetails;
}

[System.Serializable]
public class UserDetails
{
    public int USER_ID;
    public string NICKNAME;
    public int LEVEL;
    public int EXPERIENCE;
    public string INVENTORY;
}