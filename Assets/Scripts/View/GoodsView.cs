using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using TMPro;
using UnityEngine;

public class GoodsView : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI TextMesh_Gold;
    [SerializeField] TextMeshProUGUI TextMesh_Jewel;

    private GoodsViewModel _vm;

    private void OnEnable()
    {
        if (_vm == null)
        {
            _vm = new GoodsViewModel();
            _vm.PropertyChanged += OnPropertyChanged;
            _vm.RegisterEventsOnEnable_Goods(true); //true면 이벤트 ON, false면 이벤트 OFF
            _vm.RefreshViewModel_Goods();
        }
    }

    private void OnDisable()
    {
        if (_vm != null)
        {
            _vm.RegisterEventsOnEnable_Goods(false);
            _vm.PropertyChanged -= OnPropertyChanged;
            _vm = null;
        }
    }

    private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(_vm.Gold):
                TextMesh_Gold.text = _vm.Gold.ToString();
                break;
            case nameof(_vm.Jewel):
                TextMesh_Jewel.text = _vm.Jewel.ToString();
                break;
        }
    }
}
