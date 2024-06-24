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
    private Action<UserDetails> _playerInfoCallback; //플레이어 정보 처음줄 때 Callback
    private Action<int> _levelUpCallback;
    private Action<int> _expUpCallback;
    private Action<int> _goldChangedCallback;
    private Action<int> _jewelChangedCallback;

    private UserDetails _userDetails = null;

    public int GetPlayerLevel()
    {
        if(_userDetails != null)
            return _userDetails.LEVEL;
        else
            return 0;
    }


    // DB,xlsx,xml Call 문제가 발생했을 경우에, CallBack을 할 것.

    private string feedbackText;
    //private Action<int, int> _hpChangedCallback;

    [SerializeField]
    InventoryObject inventory; //밖으로 뺄 수 있게 만들 것.

    public InventoryObject Inventory
    {
        get
        {
            if (inventory == null)
                inventory.Load();
            
            return inventory;
        }
    }
    public static DataBaseManager Inst
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<DataBaseManager>();
                // [TODO : KDH]이 구간에서 XML/JSON 파일을 불러와줄 것.
                // TempInitPlayerList();
            }
            return _instance;
        }
    }
    /*private void Awake()
    {
        inventory.Load();
    }*/
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

    #region MVVM Rrfresh / Request
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
        _levelUpCallback.Invoke(_userDetails.LEVEL);
    }

    public void RequestGoldChange(int addGold)
    {
        _userDetails.GOLD += addGold;
        _goldChangedCallback.Invoke(_userDetails.GOLD);
    }

    public void RequestExpGain(int exp)
    {
        _userDetails.EXPERIENCE += exp;
        _expUpCallback.Invoke(_userDetails.EXPERIENCE);
    }

    public void RequestJewelChange(int jewel)
    {
        _userDetails.JEWEL += jewel;
        _jewelChangedCallback.Invoke(_userDetails.JEWEL);
    }

    #endregion


    #region Login Event

    /// <summary>
    /// 리퀘스트 하면 DataBaseManager.Inst.RequestLogin(string, string)을 하면 로그인 상태를 호출해서 MVVM으로 반환.
    /// </summary>
    public string RequestLogin(string username, string password)
    {
        StartCoroutine(Login(username, password));
        return feedbackText;
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
                feedbackText = "Error: " + www.error;
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
            feedbackText = "Login successful!";

            StartCoroutine(GetOrCreateUserDetails(loginResponse.userDetails.USER_ID));
        }
        else
        {
            feedbackText = "Login failed: " + loginResponse.message;
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
                feedbackText = "Error: " + www.error;
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
            feedbackText = "User details retrieved successfully!";
            InitUserDetails(userDetailsResponse);
            //RefreshPlayerInfo();
        }
        else
        {
            feedbackText = "Failed to retrieve user details: " + userDetailsResponse.message;
        }
    }

    void InitUserDetails(UserDetailsResponse user)
    {
        _userDetails = user.userDetails;
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
    public int GOLD;
    public int JEWEL;
    public string INVENTORY;
}
