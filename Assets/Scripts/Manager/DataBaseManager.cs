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

    private string feedbackText;

    [SerializeField] InventoryObject AttendItems;

    public static DataBaseManager Inst
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<DataBaseManager>();
                // [TODO : KDH] XML/JSON 파일을 불러와줄 것.
            }
            return _instance;
        }
    }

    #region Getter
    public int GetPlayerLevel()
    {
        if (_userDetails != null)
            return _userDetails.LEVEL;
        else
            return 0;
    }

    #endregion

    private void Start()
    {
        LoadInventory();
    }
    #region method
    void LoadInventory()
    {
        //AttendItems.Load();
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
        StartCoroutine(UpdateUserDetails("LEVEL", _userDetails.LEVEL, _userDetails.USER_ID));
        _levelUpCallback.Invoke(_userDetails.LEVEL);
    }

    public void RequestGoldChange(int addGold)
    {
        _userDetails.GOLD += addGold;
        _goldChangedCallback.Invoke(_userDetails.GOLD);
    }

    public void RequestExpGain(int exp)
    {
        // 경험치얻은거 req 넘은지 체크하고 넘으면 값만큼 빼주고 레벨업 적용
        // 예외처리 최고레벨일 경우 그냥 반환
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
            _expUpCallback.Invoke(_userDetails.EXPERIENCE);
        }
            
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
                feedbackText = "Error: " + www.error;
            }
            else
            {
                ProcessUserDetailsResponse(www.downloadHandler.text);
            }
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
