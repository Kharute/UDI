using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using ViewModel.Extentions;

public class WeaponSlotView : MonoBehaviour
{
    [SerializeField] Image Rarity_Backgound;        // 레어리티 -> Item이 들고있음
    [SerializeField] Image Image_Icon;              // Icon -> Item이 들고있음
    [SerializeField] TextMeshProUGUI Text_Count;    // DB에서 개수 들고 올 것.
    [SerializeField] TextMeshProUGUI Text_Tier;     // WeaponInfoList가 들고있음.

    public void SetUI(int weaponId)
    {
        var weaponInfoList = GameDataManager.Inst.WeaponInfoList;
        int _itemId = weaponInfoList[weaponId].ItemID;
        var weaponItemData = GameDataManager.Inst.GetWeaponData(_itemId);

        var weaponList = DataBaseManager.Inst.weapon_CountList;

        if (weaponItemData != null)
        {
            //Text_SkillName.text = skillData.SkillName;
            //var path = $"Textures/SkillIcons/{skillData.IconName}";
            string Rarity_path;
            switch (weaponItemData.Rarity)
            {
                case "커먼":
                case "언커먼":
                    Rarity_path = $"Icons/Tier/Grey";
                    break;
                case "레어":
                case "슈퍼레어":
                case "울트라레어":
                    Rarity_path = $"Icons/Tier/Blue";
                    break;
                case "유니크":
                case "하이퍼유니크":
                    Rarity_path = $"Icons/Tier/Purple";
                    break;
                case "에픽":
                case "얼티밋에픽":
                    Rarity_path = $"Icons/Tier/Brown";
                    break;
                case "레전더리":
                    Rarity_path = $"Icons/Tier/Red";
                    break;
                default:
                    Rarity_path = $"Icons/Tier/Grey";
                    break;
            }
            Rarity_Backgound.sprite = Resources.Load<Sprite>(Rarity_path);

            var Icon_path = $"Icons/{weaponItemData.Icon}";
            Image_Icon.sprite = Resources.Load<Sprite>(Icon_path);
            Text_Tier.text = weaponInfoList[weaponId].Tier.ToString();
            
            if (weaponList.ContainsKey(weaponId))
            {
                Text_Count.text = weaponList[weaponId].ToString();
            }
            else
            {
                Text_Count.text = "0";
            }
            
        }
    }

    public void UpdateUI()
    {
        var weaponList = DataBaseManager.Inst.weapon_CountList;
    }
}
