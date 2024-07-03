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
    [SerializeField] TextMeshProUGUI Text_Count;     // DB에서 개수 들고 올 것.
    [SerializeField] TextMeshProUGUI Text_SkillName; //

    private int _weaponId;

    public void SetUI(int WeaponID)
    {
        _weaponId = WeaponID;
        var weaponItemData = GameDataManager.Inst.GetWeaponData(_weaponId);
        // skillData -> item에서 데이터 끌고왔음.

        if (weaponItemData != null)
        {
            //Text_SkillName.text = skillData.SkillName;
            //var path = $"Textures/SkillIcons/{skillData.IconName}";
            var path = $"Icons/{weaponItemData.Icon}";  //path1은 Icon 들고오고.
            var path2 = $"Icons/{weaponItemData.Icon}"; //path2는 Tier 들고오고.

            Image_Icon.sprite = Resources.Load<Sprite>(path);
        }
    }
}
