using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;

public class DataBaseManager : MonoBehaviour
{
    private static DataBaseManager _instance = null;

    private Action<int, int> _loginCallback;
    private Action<UserDetails> _playerInfoCallback;
    private Action<int> _levelUpCallback;
    private Action<int> _expUpCallback;
    private Action<int> _goldChangedCallback;
    private Action<int> _jewelChangedCallback;

    private UserDetails _userDetails = null;
    private int _loginCountMonth;
    private bool _isFirstLogin;

    private LoginResponse login_feedback = new();
    private UserGoodsResponse goods_feedback = new();

    public static DataBaseManager Inst
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<DataBaseManager>();
            }
            return _instance;
        }
    }

    #region Getter
    public int GetPlayerLevel()
    {
        return _userDetails != null ? _userDetails.LEVEL : 0;
    }

    public int GetLoginCountMonth()
    {
        return _loginCountMonth;
    }

    public bool IsFirstLogin()
    {
        return _isFirstLogin;
    }
    #endregion

    #region MVVM Register / UnRegister CallBack
    public void RegisterLoginCallback(Action<int, int> loginCallback, bool isLogin)
    {
        if (isLogin)
            _loginCallback += loginCallback;
        else
            _loginCallback -= loginCallback;
    }

    public void RegisterExpUpCallback(Action<int> expUpCallback, bool isExpGain)
    {
        if (isExpGain)
            _expUpCallback += expUpCallback;
        else
            _expUpCallback -= expUpCallback;
    }

    public void RegisterLevelUpCallback(Action<int> levelUpCallback, bool isLevelUp)
    {
        if (isLevelUp)
            _levelUpCallback += levelUpCallback;
        else
            _levelUpCallback -= levelUpCallback;
    }

    public void RegisterGoldChangedCallback(Action<int> goldChangedCallback, bool isGold)
    {
        if (isGold)
            _goldChangedCallback += goldChangedCallback;
        else
            _goldChangedCallback -= goldChangedCallback;
    }

    public void RegisterJewelChangedCallback(Action<int> jewelChangedCallback, bool isJewel)
    {
        if (isJewel)
            _jewelChangedCallback += jewelChangedCallback;
        else
            _jewelChangedCallback -= jewelChangedCallback;
    }
    #endregion

    #region MVVM Refresh / Request
    public void RefreshPlayerInfo(Action<UserDetails> callback)
    {
        if (_userDetails != null)
        {
            callback.Invoke(_userDetails);
        }
    }

    public void RequestLevelUp()
    {
        _userDetails.LEVEL += 1;
        StartCoroutine(UpdateUserDetails("LEVEL", _userDetails.LEVEL, _userDetails.USER_ID));
        _levelUpCallback?.Invoke(_userDetails.LEVEL);
    }

    public void RequestGoldChange(int addGold)
    {
        _userDetails.GOLD += addGold;
        _goldChangedCallback?.Invoke(_userDetails.GOLD);
    }

    public void RequestExpGain(int exp)
    {
        if (_userDetails.LEVEL < GameDataManager.Inst.LevelInfoList.Count)
        {
            var _playerDetail = GameDataManager.Inst.GetPlayerDetailData(_userDetails.LEVEL);
            _userDetails.EXPERIENCE += exp;
            if (_userDetails.EXPERIENCE > _playerDetail.REQEXP)
            {
                _userDetails.EXPERIENCE -= _playerDetail.REQEXP;
                Inst.RequestLevelUp();
            }

            StartCoroutine(UpdateUserDetails("EXPERIENCE", _userDetails.EXPERIENCE, _userDetails.USER_ID));
            _expUpCallback?.Invoke(_userDetails.EXPERIENCE);
        }
    }

    public void RequestJewelChange(int jewel)
    {
        _userDetails.JEWEL += jewel;
        _jewelChangedCallback?.Invoke(_userDetails.JEWEL);
    }
    #endregion

    #region Login Event
    public LoginResponse RequestLogin(string username, string password)
    {
        StartCoroutine(Login(username, password));
        return login_feedback;
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
                login_feedback.message = "Error: " + www.error;
                login_feedback.success = false;

            }
            else
            {
                LoginResponse loginResponse = JsonUtility.FromJson<LoginResponse>(www.downloadHandler.text);

                if (loginResponse.success)
                {
                    login_feedback.message = "Login successful!";
                    login_feedback.success = true;
                    InitUserDetails(loginResponse.userDetails);
                    _loginCountMonth = loginResponse.loginCountMonth; // Save the login count month
                    _isFirstLogin = loginResponse.isFirstLogin; // Save isFirstLogin status
                    _loginCallback?.Invoke(loginResponse.userDetails.USER_ID, loginResponse.userDetails.LEVEL);
                }
                else
                {
                    login_feedback.message = "Login failed: " + loginResponse.message;
                    login_feedback.success = false;
                }
            }
        }
    }

    IEnumerator UpdateUserDetails(string column, int value, int userId)
    {
        string url = "http://localhost:3000/updateUserDetails";
        WWWForm form = new WWWForm();
        form.AddField("userId", userId);
        form.AddField("column", column);
        form.AddField("value", value);

        using (UnityWebRequest www = UnityWebRequest.Post(url, form))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                login_feedback.message = "Error: " + www.error;
                login_feedback.success = false;
            }
            else
            {
                ProcessUserDetailsResponse(www.downloadHandler.text);
            }
        }
    }

    public void OnClick_UpdateUserGoods(string column, int value)
    {
         StartCoroutine(UpdateUserGoods(column, value, _userDetails.USER_ID));
    }

    IEnumerator UpdateUserGoods(string column, int value, int userId)
    {
        string url = "http://localhost:3000/updateUserGoods";
        WWWForm form = new WWWForm();
        form.AddField("userId", userId);
        form.AddField("column", column);
        form.AddField("value", value);

        using (UnityWebRequest www = UnityWebRequest.Post(url, form))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                login_feedback.message = "Error: " + www.error;
                login_feedback.success = false;
            }
            else
            {
                ProcessUserGoodsResponse(www.downloadHandler.text);
                Invoke("", 0.5f);
            }
        }
    }

    void InitUserDetails(UserDetails userDetails)
    {
        _userDetails = userDetails;
    }

    void ProcessUserDetailsResponse(string response)
    {
        UserDetailsResponse userDetailsResponse = JsonUtility.FromJson<UserDetailsResponse>(response);

        if (userDetailsResponse.success)
        {
            login_feedback.message = "User details retrieved successfully!";
            InitUserDetails(userDetailsResponse.userDetails);
        }
        else
        {
            login_feedback.message = "Failed to retrieve user details: " + userDetailsResponse.message;
        }
    }

    void ProcessUserGoodsResponse(string response)
    {
        UserGoodsResponse userDetailsResponse = JsonUtility.FromJson<UserGoodsResponse>(response);

        if (userDetailsResponse.success)
        {
            login_feedback.message = "User goods retrieved successfully!";
        }
        else
        {
            login_feedback.message = "Failed to retrieve user goods: " + userDetailsResponse.message;
        }
    }

    #endregion
}

[System.Serializable]
public class LoginResponse
{
    public bool success;
    public string message;
    public UserDetails userDetails;
    public int loginCountMonth;
    public bool isFirstLogin; // Add this field to store isFirstLogin status
}

[System.Serializable]
public class UserDetailsResponse
{
    public bool success;
    public string message;
    public UserDetails userDetails;
}

[System.Serializable]
public class UserGoodsResponse
{
    public bool success;
    public string message;
}


[System.Serializable]
public class UserDetails
{
    public int USER_ID;
    public string NICKNAME;
    public int LEVEL;
    public int EXPERIENCE;
    public int GOLD;
    public int JEWEL;
    public string INVENTORY;
}
