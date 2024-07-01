using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;

public enum UserGoodsType
{
    GOLD,
    JEWEL,
    TICKET_WEAPON,
    TICKET_ARMOR
}

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
    private Action<UserGoodsType, int> _goldChangedCallback;
    private Action<UserGoodsType, int> _jewelChangedCallback;
    private Action<UserGoodsType, int> _ticket_weaponChangedCallback;
    private Action<UserGoodsType, int> _ticket_armorChangedCallback;

    private UserDetails _userDetails = null;
    private UserGoods _userGoods = null;
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
        switch (type)
        {
            case UserGoodsType.GOLD:
                {
                    if (value)
                        _goldChangedCallback += valueChangedCallback;
                    else
                        _goldChangedCallback -= valueChangedCallback;
                    break;
                }
            case UserGoodsType.JEWEL:
                {
                    if (value)
                        _jewelChangedCallback += valueChangedCallback;
                    else
                        _jewelChangedCallback -= valueChangedCallback;
                }
                break;
            case UserGoodsType.TICKET_WEAPON:
                {
                    if (value)
                        _ticket_weaponChangedCallback += valueChangedCallback;
                    else
                        _ticket_weaponChangedCallback -= valueChangedCallback;
                }
                break;

            case UserGoodsType.TICKET_ARMOR:
                {
                    if (value)
                        _ticket_armorChangedCallback += valueChangedCallback;
                    else
                        _ticket_armorChangedCallback -= valueChangedCallback;
                }
                break;
        }
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


    
    #region MVVM Goods

    public void RequestGoodsChange(UserGoodsType goodsType, int value)
    {
        switch(goodsType)
        {
            case UserGoodsType.GOLD:
                _userGoods.GOLD += value;
                StartCoroutine(UpdateUserGoods(goodsType, _userGoods.GOLD, _userDetails.USER_ID));
                _goldChangedCallback?.Invoke(goodsType, _userGoods.GOLD);
                break;
            case UserGoodsType.JEWEL:
                _userGoods.JEWEL += value;
                StartCoroutine(UpdateUserGoods(goodsType, _userGoods.JEWEL, _userDetails.USER_ID));
                _goldChangedCallback?.Invoke(goodsType, _userGoods.JEWEL);
                break;
            case UserGoodsType.TICKET_WEAPON:
                _userGoods.TICKET_WEAPON += value;
                StartCoroutine(UpdateUserGoods(goodsType, _userGoods.TICKET_WEAPON, _userDetails.USER_ID));
                _goldChangedCallback?.Invoke(goodsType, _userGoods.TICKET_WEAPON);
                break;
            case UserGoodsType.TICKET_ARMOR:
            _userGoods.TICKET_ARMOR += value;
                StartCoroutine(UpdateUserGoods(goodsType, _userGoods.TICKET_ARMOR, _userDetails.USER_ID));
                _goldChangedCallback?.Invoke(goodsType, _userGoods.TICKET_ARMOR);
                break;
        }
        
        //StartCoroutine(UpdateUserGoods(goodsType, _userGoods.GOLD, _userDetails.USER_ID));
        
    }

    /*// 타입을 하나 더 받아서 
    public void RequestGoldChange(int addGold)
    {
        _userGoods.GOLD += addGold;
        StartCoroutine(UpdateUserGoods(UserGoodsType.GOLD, _userGoods.GOLD, _userDetails.USER_ID));
        _goldChangedCallback?.Invoke(_userGoods.GOLD);
    }

    public void RequestJewelChange(int jewel)
    {
        _userGoods.JEWEL += jewel;
        StartCoroutine(UpdateUserGoods(UserGoodsType.GOLD, _userGoods.JEWEL, _userDetails.USER_ID));
        _jewelChangedCallback?.Invoke(_userGoods.JEWEL);
    }

    public void RequestTicketWeaponChange(int jewel)
    {
        _userGoods.JEWEL += jewel;
        _jewelChangedCallback?.Invoke(_userGoods.JEWEL);
    }

    public void RequestTicketArmorChange(int jewel)
    {
        _userGoods.JEWEL += jewel;
        _jewelChangedCallback?.Invoke(_userGoods.JEWEL);
    }*/

    #endregion MVVM Goods

    #endregion MVVM Refresh / Request

    #region Login Event
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

    void InitUserDetails(UserDetails userDetails)
    {
        _userDetails = userDetails;
    }
    void InitUserGoods(UserGoods userGoods)
    {
        _userGoods = userGoods;
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
}

[System.Serializable]
public class LoginResponse
{
    public bool success;
    public string message;
    public UserDetails userDetails;
    public UserGoods userGoods;
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

