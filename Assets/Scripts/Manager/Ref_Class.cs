using System.Collections.Generic;

[System.Serializable]
public class LoginResponse
{
    public bool success;
    public string message;
    public UserDetails userDetails;
    public UserGoods userGoods;
    public UserWeapon userWeapon;
    public int loginCountMonth;
    public bool loginIsFirst; // Add this field to store isFirstLogin status
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

