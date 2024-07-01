using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ViewModel;

public class GoodsViewModel : ViewModelBase
{
    private int _gold;
    private int _jewel;
    private int _ticket_weapon;
    private int _ticket_armor;

    public int Gold
    {
        get { return _gold; }
        set
        {
            if (_gold == value)
                return;

            _gold = value;
            OnPropertyChanged(nameof(Gold));
        }
    }

    public int Jewel
    {
        get { return _jewel; }
        set
        {
            if (_jewel == value)
                return;

            _jewel = value;
            OnPropertyChanged(nameof(Jewel));
        }
    }
    public int Ticket_Weapon
    {
        get { return _ticket_weapon; }
        set
        {
            if (_ticket_weapon == value)
                return;

            _ticket_weapon = value;
            OnPropertyChanged(nameof(Ticket_Weapon));
        }
    }
    public int Ticket_Armor
    {
        get { return _ticket_armor; }
        set
        {
            if (_ticket_armor == value)
                return;

            _ticket_armor = value;
            OnPropertyChanged(nameof(Ticket_Armor));
        }
    }

}
