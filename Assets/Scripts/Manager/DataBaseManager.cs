using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using System.Threading.Tasks;
using UnityEngine.Experimental.Rendering;
using System.Resources;
using System.Linq;

public enum LoginCase
{
    Login,
    Level,
    Exp
}
public enum UserGoodsType
{
    gold,
    jewel,
    ticket_weapon,
    ticket_armor
}
/// <summary>
/// 외부 데이터를 Save/Load 하기 위한 Manager
/// </summary>

public class DataBaseManager
{
    AppSettings appSettings;
    GameObject outGameView;
    GameObject inGameView;

    private static DataBaseManager _instance;

    private Action<string> _loginCallback;
    private Action<int> _expUpCallback;
    private Action<int> _levelUpCallback;
    private Action<UserGoodsType, int> _goodsChangedCallback;

    private UserDetails _userDetails = null;
    private UserGoods _userGoods = null;

    private int _loginCountMonth;
    private bool _isFirstLogin;

    [SerializeField]
    private LoginResponse login_feedback = new();
    private UserGoodsResponse goods_feedback = new();
    private UserWeaponResponse weapon_feedback = new();

    public Dictionary<int, int> weapon_CountList;
    public bool IsFirstLogin
    {
        get { return _isFirstLogin; }
        set { _isFirstLogin = value; }
    }

    public static DataBaseManager Inst
    {
        get
        {
            if (_instance == null)
            {
                _instance = new DataBaseManager();
            }
            return _instance;
        }
    }
    private DataBaseManager()
    {
        LoadResources();
    }

    void LoadResources()
    {
        _userGoods = new UserGoods();
        appSettings = Resources.Load<AppSettings>("AppSettings/appsettings");
        outGameView = Resources.Load<GameObject>("Prefabs/UI/OutGameUI");
        inGameView = Resources.Load<GameObject>("Prefabs/UI/InGameUI");
        Debug.Log("DB 리소스 로드 완료");
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

    #endregion

    #region MVVM Register / UnRegister CallBack

    public void RegisterLoginCallback(Action<string> loginCallback, bool isLogin)
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
    public async Task RequestLevelUp()
    {
        _userDetails.level += 1;
        await UpdateUserDetails("level", _userDetails.level, _userDetails.user_id);
        _levelUpCallback?.Invoke(_userDetails.level);
    }

    public async Task RequestExpGain(int exp)
    {
        if (_userDetails.level < GameDataManager.Inst.LevelInfoList.Count)
        {
            var _playerDetail = GameDataManager.Inst.GetPlayerDetailData(_userDetails.level);
            _userDetails.experience+= exp;
            if (_userDetails.experience> _playerDetail.REQEXP)
            {
                _userDetails.experience -= _playerDetail.REQEXP;
                await Inst.RequestLevelUp();
            }

            await UpdateUserDetails("experience", _userDetails.experience, _userDetails.user_id);
            _expUpCallback?.Invoke(_userDetails.experience);
        }
    }

    #endregion MVVM Details
    #endregion MVVM Refresh / Request


    #region Database Connection

    #region Login

    public async Task Login(string username, string password)
    {
        string url = $"{appSettings.IPAddress}:{appSettings.Port}/login";
        WWWForm form = new WWWForm();
        form.AddField("username", username);
        form.AddField("password", password);

        using (UnityWebRequest www = UnityWebRequest.Post(url, form))
        {
            var operation = www.SendWebRequest();
            while (!operation.isDone)
            {
                await Task.Yield(); // 유니티의 메인 스레드를 블로킹하지 않도록 함.
            }

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
                    await LoadWeaponData(loginResponse.userDetails.user_id);

                    _loginCountMonth = loginResponse.loginCountMonth;
                    _isFirstLogin = loginResponse.loginIsFirst;

                    //MVVM Update
                    /*_loginCallback?.Invoke(loginResponse.userDetails.nickname);
                    _expUpCallback?.Invoke(loginResponse.userDetails.experience);
                    _levelUpCallback?.Invoke(loginResponse.userDetails.level);*/

                    // Game Scene Change.
                    UIManager.Inst.GameStart();
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
    public async Task UpdateUserDetails(string column, int value, int userId)
    {   
        string url = $"{appSettings.IPAddress}:{appSettings.Port}/updateUserDetails";

        WWWForm form = new WWWForm();
        form.AddField("userId", userId);
        form.AddField("column", column);
        form.AddField("value", value);


        Debug.Log($"column : {column}, value : {value}, UserID : {userId}");
        using (UnityWebRequest www = UnityWebRequest.Post(url, form))
        {
            var operation = www.SendWebRequest();
            while (!operation.isDone)
            {
                await Task.Yield(); // 유니티의 메인 스레드를 블로킹하지 않도록 함.
            }

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                login_feedback.message = "Error: " + www.error;
                login_feedback.success = false;
            }
        }
    }

    void InitUserDetails(UserDetails userDetails)
    {
        _userDetails = userDetails;
    }

    #endregion

    #region UserGoods

    public async Task RequestGoodsChange(UserGoodsType goodsType, int value)
    {
        switch (goodsType)
        {
            case UserGoodsType.gold:
                value += _userGoods.gold;
                break;
            case UserGoodsType.jewel:
                value += _userGoods.jewel;
                break;
            case UserGoodsType.ticket_weapon:
                value += _userGoods.ticket_weapon;
                break;
            case UserGoodsType.ticket_armor:
                value += _userGoods.ticket_armor;
                break;
        }

        await UpdateUserGoods(goodsType, value, _userDetails.user_id);
        _goodsChangedCallback?.Invoke(goodsType, value);
    }

    public async Task UpdateUserGoods(UserGoodsType column, int value, int userId)
    {
        string url = $"{appSettings.IPAddress}:{appSettings.Port}/updateUserGoods";
        WWWForm form = new WWWForm();

        form.AddField("userId", userId);
        form.AddField("column", column.ToString()); //nameof 체크할 것.
        form.AddField("value", value);

        Debug.Log($"column : {column}, value : {value}, UserID : {userId}");
        using (UnityWebRequest www = UnityWebRequest.Post(url, form))
        {
            var operation = www.SendWebRequest();
            while (!operation.isDone)
            {
                await Task.Yield(); // 유니티의 메인 스레드를 블로킹하지 않도록 함.
            }

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

    async Task LoadWeaponData(int userId)
    {
        string url = $"{appSettings.IPAddress}:{appSettings.Port}/loadWeaponData";
        WWWForm form = new WWWForm();

        form.AddField("userId", userId);

        using (UnityWebRequest www = UnityWebRequest.Post(url, form))
        {
            var operation = www.SendWebRequest();
            while (!operation.isDone)
            {
                await Task.Yield(); // 유니티의 메인 스레드를 블로킹하지 않도록 함.
            }
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

    public async Task BindWeapon()
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
            await UploadWeaponData();
        }
    }

    private async Task UploadWeaponData()
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

        var operation = www.SendWebRequest();
        while (!operation.isDone)
        {
            await Task.Yield(); // 유니티의 메인 스레드를 블로킹하지 않도록 함.
        }

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

    public async Task RequestGacha(int count, Action<Dictionary<int, int>> callback)
    {
        await RequestGachaAsync(_userDetails.user_id, count, callback);
    }
    
    private async Task RequestGachaAsync(int userId, int count, Action<Dictionary<int, int>> callback)
    {
        string url = $"{appSettings.IPAddress}:{appSettings.Port}/gacha";

        WWWForm form = new WWWForm();
        form.AddField("userId", userId);
        form.AddField("count", count);

        using (UnityWebRequest www = UnityWebRequest.Post(url, form))
        {
            var operation = www.SendWebRequest();
            while (!operation.isDone)
            {
                await Task.Yield(); // 유니티의 메인 스레드를 블로킹하지 않도록 함.
            }

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(www.error);
            }
            else
            {
                try
                {
                    string jsonResponse = www.downloadHandler.text;
                    GachaResponse response = JsonConvert.DeserializeObject<GachaResponse>(jsonResponse);
                    Dictionary<int, int> result = response.Result.ToDictionary(entry => int.Parse(entry.Key), entry => entry.Value);

                    ProcessGachaResponse(result);
                    callback?.Invoke(result);
                }
                catch(Exception e)
                {
                    Debug.LogError(e.Message);
                }

            }
        }
    }

    private void ProcessGachaResponse(Dictionary<int, int> response)
    {
        foreach (var item in response)
        {
            int parseInt = item.Key;
            if (weapon_CountList.ContainsKey(parseInt))
            {
                weapon_CountList[parseInt] += item.Value;
            }
            else
            {
                weapon_CountList[parseInt] = item.Value;
            }
        }
    }

    #endregion

    #endregion Database Connection

}
public class GachaResponse
{
    public bool Success { get; set; }
    public Dictionary<string, int> Result { get; set; }
}

