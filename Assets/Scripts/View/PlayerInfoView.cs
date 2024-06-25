using System.ComponentModel;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInfoView : MonoBehaviour
{
    // GameDataManager���� ���� �ҷ������ ��.

    [SerializeField] TextMeshProUGUI TextMesh_Name;
    [SerializeField] TextMeshProUGUI TextMesh_level;
    [SerializeField] TextMeshProUGUI TextMesh_exp;
    
    [SerializeField] Slider Slider_exp;
        
    private PlayerInfoViewModel _vm;

    //�������ϸ� ����ġ�ٵ� �ʱ�ȭ �ؾߵ�.
    //InitSliderBar() 
    private void OnEnable()
    {
        if (_vm == null)
        {
            _vm = new PlayerInfoViewModel();
            _vm.PropertyChanged += OnPropertyChanged;
            _vm.RegisterEventsOnEnable(true); //true�� �̺�Ʈ ON, false�� �̺�Ʈ OFF
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
        var _detailData = GameDataManager.Inst.GetPlayerDetailData(_vm.Level);

        if (_detailData != null)
        {
            switch (e.PropertyName)
            {
                case nameof(_vm.Name):
                    TextMesh_Name.text = _vm.Name;
                    break;
                case nameof(_vm.Level):
                    TextMesh_level.text = _vm.Level.ToString();
                    if (Slider_exp != null)
                        Slider_exp.maxValue = _detailData.REQEXP;
                    break;
                case nameof(_vm.Exp):
                    TextMesh_exp.text = $"{_vm.Exp} / {_detailData.REQEXP} exp";
                    Slider_exp.value = _vm.Exp;
                    break;
            }
        }
        
    }
}
