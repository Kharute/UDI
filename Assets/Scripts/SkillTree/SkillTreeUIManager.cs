using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class SkillTreeUIManager : MonoBehaviour
{
    public SkillTreeInitializer skillTreeInitializer;
    public GameObject skillButtonPrefab;
    public Transform skillButtonContainer;

    void Start()
    {
        PopulateSkillTree();
    }

    void PopulateSkillTree()
    {
        foreach (var skill in skillTreeInitializer.skillTree.Skills)
        {
            GameObject skillButtonObject = Instantiate(skillButtonPrefab, skillButtonContainer);
            SkillButton skillButton = skillButtonObject.GetComponent<SkillButton>();
            skillButton.Initialize(skill);
        }
    }
}
