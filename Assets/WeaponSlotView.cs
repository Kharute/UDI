using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using ViewModel.Extentions;

public class WeaponSlotView : MonoBehaviour
{
    [SerializeField] Image Rarity_Backgound;        // ���Ƽ -> Item�� �������
    [SerializeField] Image Image_Icon;              // Icon -> Item�� �������
    [SerializeField] TextMeshProUGUI Text_Count;    // DB���� ���� ��� �� ��.
    [SerializeField] TextMeshProUGUI Text_Tier;     // WeaponInfoList�� �������.

    private int _weaponId;

    public void SetUI(int WeaponId)
    {
        var weaponInfoList = GameDataManager.Inst.WeaponInfoList;
        _weaponId = WeaponId;
        var weaponItemData = GameDataManager.Inst.GetWeaponData(_weaponId);
        // skillData -> item���� ������ �������.
        
        if (weaponItemData != null)
        {
            //
            //Text_SkillName.text = skillData.SkillName;
            //var path = $"Textures/SkillIcons/{skillData.IconName}";
            string path;
            switch (weaponItemData.Rarity)
            {
                case "Ŀ��":
                case "��Ŀ��":
                    path = $"Icons/Tier/Grey";
                    break;
                case "����":
                case "���۷���":
                case "��Ʈ�󷹾�":
                    path = $"Icons/Tier/Blue";
                    break;
                case "����ũ":
                case "����������ũ":
                    path = $"Icons/Tier/Purple";
                    break;
                case "����":
                case "��Ƽ�Կ���":
                    path = $"Icons/Tier/Brown";
                    break;
                case "��������":
                    path = $"Icons/Tier/Red";
                    break;
                default:
                    path = $"Icons/Tier/Gray";
                    break;
            }
            //var path = $"Icons/{weaponItemData.Icon}"; //path2�� Tier ������.

            Image_Icon.sprite = Resources.Load<Sprite>(path);


        }
    }
}
