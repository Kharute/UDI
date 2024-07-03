using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using ViewModel.Extentions;

public class WeaponSlotView : MonoBehaviour
{
    [SerializeField] Image Tier_Backgound;
    [SerializeField] Image Image_Icon;
    [SerializeField] TextMeshProUGUI Text_Count;     // DB���� ���� ��� �� ��.
    [SerializeField] TextMeshProUGUI Text_SkillName; //

    private int _weaponId;

    public void SetUI(int WeaponID)
    {
        _weaponId = WeaponID;
        var weaponItemData = GameDataManager.Inst.GetWeaponData(_weaponId);
        // skillData -> item���� ������ �������.

        if (weaponItemData != null)
        {
            //Text_SkillName.text = skillData.SkillName;
            //var path = $"Textures/SkillIcons/{skillData.IconName}";
            var path = $"Icons/{weaponItemData.Icon}";  //path1�� Icon ������.
            var path2 = $"Icons/{weaponItemData.Icon}"; //path2�� Tier ������.

            Image_Icon.sprite = Resources.Load<Sprite>(path);
        }
    }
}
