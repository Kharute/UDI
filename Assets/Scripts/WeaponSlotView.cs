using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using ViewModel.Extentions;

public class WeaponSlotView : MonoBehaviour
{
    [SerializeField] Image Rarity_Backgound;        // ���Ƽ -> Item�� �������
    [SerializeField] Image Image_Icon;              // Icon -> Item�� �������
    [SerializeField] TextMeshProUGUI Text_Count;    // DB���� ���� ��� �� ��.
    [SerializeField] TextMeshProUGUI Text_Tier;     // WeaponInfoList�� �������.

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
                case "Ŀ��":
                case "��Ŀ��":
                    Rarity_path = $"Icons/Tier/Grey";
                    break;
                case "����":
                case "���۷���":
                case "��Ʈ�󷹾�":
                    Rarity_path = $"Icons/Tier/Blue";
                    break;
                case "����ũ":
                case "����������ũ":
                    Rarity_path = $"Icons/Tier/Purple";
                    break;
                case "����":
                case "��Ƽ�Կ���":
                    Rarity_path = $"Icons/Tier/Brown";
                    break;
                case "��������":
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
