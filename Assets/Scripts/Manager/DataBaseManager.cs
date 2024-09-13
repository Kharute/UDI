using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

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
    [SerializeField] AppSettings appSettings;

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

    [SerializeField]
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
        return _userDetails != null ? _userDetails.level : 0;
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
        _userDetails.level += 1;
        StartCoroutine(UpdateUserDetails("level", _userDetails.level, _userDetails.user_id));
        _levelUpCallback?.Invoke(_userDetails.level);
    }

    public void RequestExpGain(int exp)
    {
        if (_userDetails.level < GameDataManager.Inst.LevelInfoList.Count)
        {
            var _playerDetail = GameDataManager.Inst.GetPlayerDetailData(_userDetails.level);
            _userDetails.experience+= exp;
            if (_userDetails.experience> _playerDetail.REQEXP)
            {
                _userDetails.experience -= _playerDetail.REQEXP;
                Inst.RequestLevelUp();
            }

            StartCoroutine(UpdateUserDetails("experience", _userDetails.level, _userDetails.user_id));
            _expUpCallback?.Invoke(_userDetails.experience);
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
        string url = $"{appSettings.IPAddress}:{appSettings.Port}/login";
        WWWForm form = new WWWForm();
        form.AddField("username", username);
        form.AddField("password", password);

        using (UnityWebRequest www = UnityWebRequest.Post(url, form))
        {
            yield return www.SendWebRequest();
            /*Debug.Log($"www.result : {www.responseCode}");
            Debug.Log($"www.result : {www.result}");*/
            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                login_feedback.message = "Error: " + www.error;
                login_feedback.success = false;
                Debug.Log("login error");
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
                    RequestLoadWeaponData(loginResponse.userDetails.user_id);

                    _loginCountMonth = loginResponse.loginCountMonth;
                    _isFirstLogin = loginResponse.isFirstLogin;
                    _loginCallback?.Invoke(loginResponse.userDetails.user_id, loginResponse.userDetails.level);

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
        string url = $"{appSettings.IPAddress}:{appSettings.Port}/updateUserDetails";

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
                value += _userGoods.gold;
                break;
            case UserGoodsType.JEWEL:
                value += _userGoods.jewel;
                break;
            case UserGoodsType.TICKET_WEAPON:
                value += _userGoods.ticket_weapon;
                break;
            case UserGoodsType.TICKET_ARMOR:
                value += _userGoods.ticket_armor;
                break;
        }

        StartCoroutine(UpdateUserGoods(goodsType, value, _userDetails.user_id));
        _goodsChangedCallback?.Invoke(goodsType, value);
    }

    IEnumerator UpdateUserGoods(UserGoodsType column, int value, int userId)
    {
        string url = $"{appSettings.IPAddress}:{appSettings.Port}/updateUserGoods";
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
        string url = $"{appSettings.IPAddress}:{appSettings.Port}/loadWeaponData";

        WWWForm form = new WWWForm();

        form.AddField("userId", userId);

        using (UnityWebRequest www = UnityWebRequest.Post(url, form))
        {
            yield return www.SendWebRequest();
            Debug.Log($"www.result : {www.responseCode}");
            Debug.Log($"www.result : {www.result}");

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
                weapon_CountList.Add(weapon.weapon_id, weapon.weapon_count);
            }
        }
    }

    public void BindWeapon()
    {
        bool isChanged = false;
        var weaponsToUpdate = new Dictionary<int, int>();
        var weaponsToAdd = new Dictionary<int, int>();

        foreach (var weapon in weapon_CountList)
        {
            if (weapon.Value >= 5)
            {
                isChanged = true;
                int newCount = weapon.Value - 5;
                weaponsToUpdate[weapon.Key] = newCount;

                int upgradeWeaponId = weapon.Key + 1;
                if (weapon_CountList.ContainsKey(upgradeWeaponId))
                {
                    weaponsToUpdate[upgradeWeaponId] = weapon_CountList[upgradeWeaponId] + 1;
                }
                else
                {
                    weaponsToAdd[upgradeWeaponId] = 1;
                }
            }
        }

        // 변경사항 적용
        foreach (var update in weaponsToUpdate)
        {
            weapon_CountList[update.Key] = update.Value;
        }

        foreach (var add in weaponsToAdd)
        {
            weapon_CountList.Add(add.Key, add.Value);
        }

        if (isChanged)
        {
            UploadUserWeaponData();
        }
    }

    public void UploadUserWeaponData()
    {
        StartCoroutine(UploadWeaponData());
    }

    private IEnumerator UploadWeaponData()
    {
        string url = $"{appSettings.IPAddress}:{appSettings.Port}/uploadWeaponData";

        var weaponData = new List<WeaponData>();
        foreach (var kvp in weapon_CountList)
        {
            weaponData.Add(new WeaponData
            {
                weapon_id = kvp.Key,
                weapon_count = kvp.Value
            });
        }

        var data = new WeaponRequest
        {
            user_id = _userDetails.user_id,
            weapons = weaponData
        };

        string jsonData = JsonUtility.ToJson(data);

        UnityWebRequest www = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
        www.uploadHandler = new UploadHandlerRaw(bodyRaw);
        www.downloadHandler = new DownloadHandlerBuffer();
        www.SetRequestHeader("Content-Type", "application/json");

        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Upload successful");
        }
        else
        {
            Debug.LogError("Error: " + www.error);
        }
    }

    #endregion UserWeapon

    #region Gacha System

    public void RequestGacha(int count, Action<Dictionary<int, int>> callback)
    {
        StartCoroutine(RequestGachaCoroutine(_userDetails.user_id, count, callback));
    }
    
    private IEnumerator RequestGachaCoroutine(int userId, int count, Action<Dictionary<int, int>> callback)
    {
        string url = $"{appSettings.IPAddress}:{appSettings.Port}/gacha";

        WWWForm form = new WWWForm();
        form.AddField("userId", userId);
        form.AddField("count", count);

        using (UnityWebRequest www = UnityWebRequest.Post(url, form))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(www.error);
            }
            else
            {
                string jsonResponse = www.downloadHandler.text;
                Dictionary<int, int> response = JsonConvert.DeserializeObject<Dictionary<int, int>>(jsonResponse);
                ProcessGachaResponse(response);
                callback?.Invoke(response);
            }
        }
    }

    private void ProcessGachaResponse(Dictionary<int, int> response)
    {
        foreach (var item in response)
        {
            if (weapon_CountList.ContainsKey(item.Key))
            {
                weapon_CountList[item.Key] += item.Value;
            }
            else
            {
                weapon_CountList[item.Key] = item.Value;
            }
        }
    }

    #endregion

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
    public int user_id;
    public string nickname;
    public int level;
    public int experience;
    public int skill_point;
    public string inventory;
}

[System.Serializable]
public class UserGoods
{
    public int user_id;
    public int gold;
    public int jewel;
    public int ticket_weapon;
    public int ticket_armor;
}

[System.Serializable]
public class UserWeapon
{
    public int weapon_id;
    public int weapon_count;
}

[System.Serializable]
public class WeaponRequest
{
    public int user_id;
    public List<WeaponData> weapons;
}

[System.Serializable]
public class WeaponData
{
    public int weapon_id;
    public int weapon_count;
}

