using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;
using System.Collections.Generic;

public enum UserGoodsType
{
    GOLD,
    JEWEL,
    TICKET_WEAPON,
    TICKET_ARMOR
}
/// <summary>
/// 외부 데이터를 Save/Load 하기 위한 Manager
/// </summary>
public class DataBaseManager : MonoBehaviour
{
    [SerializeField]
    OutGameView outGameView;

    [SerializeField]
    InGameView inGameView;

    private static DataBaseManager _instance = null;

    private Action<int, int> _loginCallback;
    private Action<UserDetails> _playerInfoCallback;
    private Action<int> _levelUpCallback;
    private Action<int> _expUpCallback;
    private Action<UserGoodsType, int> _goodsChangedCallback;

    private UserDetails _userDetails = null;
    private UserGoods _userGoods = null;
    private UserWeapon _userWeapon = null;

    private int _loginCountMonth;
    private bool _isFirstLogin;

    private LoginResponse login_feedback = new();
    private UserGoodsResponse goods_feedback = new();
    private UserWeaponResponse weapon_feedback = new();

    public Dictionary<int, int> weapon_CountList;

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

    private void Awake()
    {
        _userGoods = new UserGoods();
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

    public void Register_GoodsChangedCallback(Action<UserGoodsType, int> valueChangedCallback, UserGoodsType type, bool value)
    {
        if (value)
            _goodsChangedCallback += valueChangedCallback;
        else
            _goodsChangedCallback -= valueChangedCallback;
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
    public void RefreshPlayerInfo(Action<UserGoods> callback)
    {
        if (_userGoods != null)
        {
            callback.Invoke(_userGoods);
        }
    }

    #region MVVM Details
    public void RequestLevelUp()
    {
        _userDetails.LEVEL += 1;
        StartCoroutine(UpdateUserDetails("LEVEL", _userDetails.LEVEL, _userDetails.USER_ID));
        _levelUpCallback?.Invoke(_userDetails.LEVEL);
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

    #endregion MVVM Details

    #endregion MVVM Refresh / Request

    #region Database Connection

    #region Login
    public void RequestLogin(string username, string password)
    {
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

                    // Data Load
                    InitUserDetails(loginResponse.userDetails);
                    InitUserGoods(loginResponse.userGoods);
                    RequestLoadWeaponData(loginResponse.userDetails.USER_ID);

                    _loginCountMonth = loginResponse.loginCountMonth;
                    _isFirstLogin = loginResponse.isFirstLogin;
                    _loginCallback?.Invoke(loginResponse.userDetails.USER_ID, loginResponse.userDetails.LEVEL);

                    // Game Scene Change.
                    outGameView.OnDisable_Login();
                    inGameView.OnStart_GameUI();
                }
                else
                {
                    login_feedback.message = "Login failed: " + loginResponse.message;
                    login_feedback.success = false;
                }
            }
        }
    }
    #endregion Login

    #region UserDetails
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

    void InitUserDetails(UserDetails userDetails)
    {
        _userDetails = userDetails;
    }

    #endregion

    #region UserGoods

    public void RequestGoodsChange(UserGoodsType goodsType, int value)
    {
        switch (goodsType)
        {
            case UserGoodsType.GOLD:
                value += _userGoods.GOLD;
                break;
            case UserGoodsType.JEWEL:
                value += _userGoods.JEWEL;
                break;
            case UserGoodsType.TICKET_WEAPON:
                value += _userGoods.TICKET_WEAPON;
                break;
            case UserGoodsType.TICKET_ARMOR:
                value += _userGoods.TICKET_ARMOR;
                break;
        }

        StartCoroutine(UpdateUserGoods(goodsType, value, _userDetails.USER_ID));
        _goodsChangedCallback?.Invoke(goodsType, value);
    }

    IEnumerator UpdateUserGoods(UserGoodsType column, int value, int userId)
    {
        string url = "http://localhost:3000/updateUserGoods";
        WWWForm form = new WWWForm();

        form.AddField("userId", userId);
        form.AddField("column", column.ToString()); //nameof 체크할 것.
        form.AddField("value", value);

        using (UnityWebRequest www = UnityWebRequest.Post(url, form))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                goods_feedback.message = "Error: " + www.error;
                goods_feedback.success = false;
            }
            else
            {
                login_feedback.success = true;
                ProcessUserGoodsResponse(www.downloadHandler.text);
            }
        }
    }

    void InitUserGoods(UserGoods userGoods)
    {
        _userGoods = userGoods;
    }

    void ProcessUserGoodsResponse(string response)
    {
        UserGoodsResponse userGoodsResponse = JsonUtility.FromJson<UserGoodsResponse>(response);

        if (userGoodsResponse.success)
        {
            goods_feedback.message = "User goods retrieved successfully!";
            InitUserGoods(userGoodsResponse.userGoods);
        }
        else
        {
            goods_feedback.message = "Failed to retrieve user goods: " + userGoodsResponse.message;
        }
    }
    #endregion

    #region UserWeapon
    public void RequestLoadWeaponData(int userId)
    {
        StartCoroutine(LoadWeaponData(userId));
    }

    IEnumerator LoadWeaponData(int userId)
    {
        string url = "http://localhost:3000/loadWeaponData";
        WWWForm form = new WWWForm();

        form.AddField("userId", userId);

        using (UnityWebRequest www = UnityWebRequest.Post(url, form))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError(www.error);
            }
            else
            {
                string json = www.downloadHandler.text;
                string wrappedJson = "{\"userWeapon\":" + json + "}";
                weapon_feedback = JsonUtility.FromJson<UserWeaponResponse>(wrappedJson);
                InitWeaponCount(weapon_feedback);
            }
        }
    }

    void InitUserWeapon(UserWeapon userWeapon)
    {
        _userWeapon = userWeapon;
    }

    void InitWeaponCount(UserWeaponResponse weapon_feedback)
    {
        if(weapon_CountList == null)
        {
            weapon_CountList = new();
            foreach (var weapon in weapon_feedback.userWeapon)
            {
                weapon_CountList.Add(weapon.WeaponID, weapon.WeaponCount);
            }
        }
    }

    void BindWeapon()
    {
        foreach(var weapon in weapon_CountList)
        {
            if(weapon.Value > 5)
            {
                weapon_CountList[weapon.Key] -= 5;

                if(weapon_CountList.ContainsKey(weapon.Key+1))
                {
                    weapon_CountList[weapon.Key + 1] += 1;
                }
                else
                {
                    weapon_CountList.Add(weapon.Key, 1);
                }
            }
        }
        //[todo] 이러고 DB에다가 ++하면 됨
    }
    IEnumerator UpdateUserWeapon(int weaponId, int value, int userId)
    {
        string url = "http://localhost:3000/updateUserGoods";
        WWWForm form = new WWWForm();

        form.AddField("userId", userId);
        form.AddField("weaponId", weaponId);
        form.AddField("value", value);

        using (UnityWebRequest www = UnityWebRequest.Post(url, form))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                /*weapon_feedback.message = "Error: " + www.error;
                weapon_feedback.success = false;*/
            }
            else
            {
                //weapon_feedback.success = true;
                ProcessUserGoodsResponse(www.downloadHandler.text);
            }
        }
    }

    #endregion UserWeapon

    #endregion Database Connection
}

[System.Serializable]
public class LoginResponse
{
    public bool success;
    public string message;
    public UserDetails userDetails;
    public UserGoods userGoods;
    public UserWeapon userWeapon;
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
    public UserGoods userGoods;
}

[System.Serializable]
public class UserWeaponResponse
{
    public List<UserWeapon> userWeapon;
}


[System.Serializable]
public class UserDetails
{
    public int USER_ID;
    public string NICKNAME;
    public int LEVEL;
    public int EXPERIENCE;
    public int SKILL_POINT;
    public string INVENTORY;
}

[System.Serializable]
public class UserGoods
{
    public int USER_ID;
    public int GOLD;
    public int JEWEL;
    public int TICKET_WEAPON;
    public int TICKET_ARMOR;
}

[System.Serializable]
public class UserWeapon
{
    public int WeaponID;
    public int WeaponCount;
}


