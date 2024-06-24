using UnityEngine;

public class SkillTreeInitializer : MonoBehaviour
{
    public SkillTree skillTree;

    void Start()
    {
        skillTree = new SkillTree();

        Skill skillA = new Skill("Skill A", "Description for Skill A");
        Skill skillB = new Skill("Skill B", "Description for Skill B");
        Skill skillC = new Skill("Skill C", "Description for Skill C");

        skillB.Prerequisites.Add(skillA);
        skillC.Prerequisites.Add(skillB);

        skillTree.AddSkill(skillA);
        skillTree.AddSkill(skillB);
        skillTree.AddSkill(skillC);
    }
}
