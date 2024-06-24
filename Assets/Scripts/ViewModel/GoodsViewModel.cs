using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ViewModel;

public class GoodsViewModel : ViewModelBase
{
    private int _gold;
    private int _jewel;

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
}
