using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInfoView : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI TextMesh_Name;
    [SerializeField] TextMeshProUGUI TextMesh_level;
    [SerializeField] TextMeshProUGUI TextMesh_exp;
    
    [SerializeField] Slider Slider_exp;
        
    private PlayerInfoViewModel _vm;

    
    private void OnEnable()
    {
        if (_vm == null)
        {
            _vm = new PlayerInfoViewModel();
            _vm.PropertyChanged += OnPropertyChanged;
            _vm.RegisterEventsOnEnable(true); //true면 이벤트 ON, false면 이벤트 OFF
            _vm.RefreshViewModel_PlayerInfo();
        }
    }

    private void OnDisable()
    {
        if (_vm != null)
        {
            _vm.RegisterEventsOnEnable(false);
            _vm.PropertyChanged -= OnPropertyChanged;
            _vm = null;
        }
    }

    private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(_vm.Name):
                TextMesh_Name.text = _vm.Name;
                break;
            case nameof(_vm.Level):
                TextMesh_level.text = _vm.Level.ToString();
                break;
            case nameof(_vm.Exp):
                TextMesh_exp.text = _vm.Exp.ToString();
                Slider_exp.value = _vm.Exp;
                break;
        }
    }
}
