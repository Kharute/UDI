using UnityEngine;
using UnityEngine.UI;

public class SkillButton : MonoBehaviour
{
    public Text skillNameText;
    public Button unlockButton;
    private Skill skill;

    public void Initialize(Skill skill)
    {
        this.skill = skill;
        skillNameText.text = skill.Name;
        unlockButton.onClick.AddListener(UnlockSkill);
        UpdateButton();
    }

    private void UnlockSkill()
    {
        skill.Unlock();
        UpdateButton();
    }

    private void UpdateButton()
    {
        unlockButton.interactable = skill.CanUnlock() && !skill.IsUnlocked;
    }
}
