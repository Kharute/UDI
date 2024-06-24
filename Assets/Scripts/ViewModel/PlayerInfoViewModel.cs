using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ViewModel;

public class PlayerInfoViewModel : ViewModelBase
{
    private string _userName;
    private int _level;
    private int _exp;


    public string Name
    {
        get { return _userName; }
        set
        {
            if (_userName == value)
                return;

            _userName = value;
            OnPropertyChanged(nameof(Name));
        }
    }

    public int Level
    {
        get { return _level; }
        set
        {
            if (_level == value)
                return;

            _level = value;
            OnPropertyChanged(nameof(Level));
        }
    }

    public int Exp
    {
        get { return _exp; }
        set
        {
            if (_exp == value)
                return;

            _exp = value;
            OnPropertyChanged(nameof(Exp));
        }
    }
}
