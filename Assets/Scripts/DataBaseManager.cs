using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System;
using TMPro;

public class DataBaseManager : MonoBehaviour
{
    //[TODO : KDH] �ش� ���ڿ� ó�� ���Դٸ� �α����г��� ����� ��.

    private static DataBaseManager _instance = null;
    //private int _curSelectedPlayerId = 0;

    //private static Dictionary<int, Player> _playerDic = new Dictionary<int, Player>();
    private Action<int, int> _loginCallback;

    private UserDetails _userDetails;
    // DB,xlsx,xml Call ������ �߻����� ��쿡, CallBack�� �� ��.

    [SerializeField]
    TextMeshProUGUI feedbackText;
    //private Action<int, int> _hpChangedCallback;

    // ���ο��� ���ư������.
    public static DataBaseManager Inst
    {
        get
        {
            if (_instance == null)
            {
                _instance = new DataBaseManager();

                //[TODO : KDH]�� �������� XML/JSON ������ �ҷ����� ��.
                //TempInitPlayerList();
            }
            return _instance;
        }
    }

    public static void TempInitPlayerList()
    {
        
    }

    #region  CallBack

    public void RegisterLoginCallback(Action<int, int> loginCallback, bool isLogin)
    {
        if (isLogin)
            _loginCallback += loginCallback;
        else
            _loginCallback -= loginCallback;
    }

    #endregion


    #region Login Event

    /// <summary>
    /// ������Ʈ �ϸ� DataBaseManager.Inst.RequestLogin(string, string)�� �ϸ� �α��� ���¸� ȣ���ؼ� MVVM���� ��ȯ.
    /// </summary>
    public string RequestLogin(string username, string password)
    {
        StartCoroutine(Login(username, password));
        return feedbackText.text;
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
            InitUserDetails(userDetailsResponse);

        }
        else
        {
            feedbackText.text = "Failed to retrieve user details: " + userDetailsResponse.message;
        }
    }

    void InitUserDetails(UserDetailsResponse user)
    {
        _userDetails.USER_ID = user.userDetails.USER_ID;
        _userDetails.NICKNAME = user.userDetails.NICKNAME;
        _userDetails.LEVEL = user.userDetails.LEVEL;
        _userDetails.EXPERIENCE = user.userDetails.EXPERIENCE;
        _userDetails.INVENTORY = user.userDetails.INVENTORY;
    }
    #endregion
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
