using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using ViewModel.Extentions;

public class WeaponSlotView : MonoBehaviour
{
    [SerializeField] Image Rarity_Backgound;        // 레어리티 -> Item이 들고있음
    [SerializeField] Image Image_Icon;              // Icon -> Item이 들고있음
    [SerializeField] TextMeshProUGUI Text_Count;    // DB에서 개수 들고 올 것.
    [SerializeField] TextMeshProUGUI Text_Tier;     // WeaponInfoList가 들고있음.

    private int _weaponId;

    public void SetUI(int WeaponId)
    {
        var weaponInfoList = GameDataManager.Inst.WeaponInfoList;
        _weaponId = WeaponId;
        var weaponItemData = GameDataManager.Inst.GetWeaponData(_weaponId);
        // skillData -> item에서 데이터 끌고왔음.
        
        if (weaponItemData != null)
        {
            //
            //Text_SkillName.text = skillData.SkillName;
            //var path = $"Textures/SkillIcons/{skillData.IconName}";
            string path;
            switch (weaponItemData.Rarity)
            {
                case "커먼":
                case "언커먼":
                    path = $"Icons/Tier/Grey";
                    break;
                case "레어":
                case "슈퍼레어":
                case "울트라레어":
                    path = $"Icons/Tier/Blue";
                    break;
                case "유니크":
                case "하이퍼유니크":
                    path = $"Icons/Tier/Purple";
                    break;
                case "에픽":
                case "얼티밋에픽":
                    path = $"Icons/Tier/Brown";
                    break;
                case "레전더리":
                    path = $"Icons/Tier/Red";
                    break;
                default:
                    path = $"Icons/Tier/Gray";
                    break;
            }
            //var path = $"Icons/{weaponItemData.Icon}"; //path2는 Tier 들고오고.

            Image_Icon.sprite = Resources.Load<Sprite>(path);


        }
    }
}
