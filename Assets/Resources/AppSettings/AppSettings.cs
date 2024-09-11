using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "appsettings", menuName = "Scriptable Object/App Settings")]

public class AppSettings : ScriptableObject
{
    [SerializeField]
    private string _IPAddress;

    [SerializeField]
    private int _port;

    public string IPAddress
    {
        get { return _IPAddress; }
    }
    public int Port
    {
        get { return _port; }
    }
}
