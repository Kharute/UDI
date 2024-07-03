using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using ViewModel.Extentions;
using static UnityEditor.Progress;

public class SkillSlotView : MonoBehaviour
{
    [SerializeField] Button Icon_btn;
    [SerializeField] Image Image_Icon;
    [SerializeField] TextMeshProUGUI Text_SkillName;

    private string _skillClassName;

    public void SetUI(string skillClassName)
    {
        _skillClassName = skillClassName;

        var skillData = GameDataManager.Inst.GetSkillData(_skillClassName);
        if (skillData != null)
        {
            Text_SkillName.text = skillData.SkillName;
            //var path = $"Textures/SkillIcons/{skillData.IconName}";
            var path = $"Icons/{skillData.Icon}";
            Image_Icon.sprite = Resources.Load<Sprite>(path);
        }
    }

    /*public void OnClick_OpenTooltip()
    {
        var skillData = GameDataManager.Inst.GetSkillData(_skillClassName);
        if (skillData == null)
            return;

        UIManager.Instance.OpenTooltipPopup(string.Format(skillData.Description, skillData.BaseDamage));
    }*/
}
