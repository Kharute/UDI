using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System;
using TMPro;

public class DataBaseManager : MonoBehaviour
{
    //[TODO : KDH] 해당 일자에 처음 들어왔다면 로그인패널을 띄워줄 것.

    private static DataBaseManager _instance = null;
    //private int _curSelectedPlayerId = 0;

    //private static Dictionary<int, Player> _playerDic = new Dictionary<int, Player>();
    private Action<int, int> _loginCallback;

    private UserDetails _userDetails;
    // DB,xlsx,xml Call 문제가 발생했을 경우에, CallBack을 할 것.

    [SerializeField]
    TextMeshProUGUI feedbackText;
    //private Action<int, int> _hpChangedCallback;

    // 내부에서 돌아가줘야함.
    public static DataBaseManager Inst
    {
        get
        {
            if (_instance == null)
            {
                _instance = new DataBaseManager();

                //[TODO : KDH]이 구간에서 XML/JSON 파일을 불러와줄 것.
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
    /// 리퀘스트 하면 DataBaseManager.Inst.RequestLogin(string, string)을 하면 로그인 상태를 호출해서 MVVM으로 반환.
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
