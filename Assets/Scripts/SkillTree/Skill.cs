using System.Collections.Generic;

public enum SkillType
{
    Active,
    Passive
}

public class Skill
{
    public string SkillName { get; set; }
    public string Description { get; set; }
    public SkillType Type { get; set; }
    public int Value { get; set; }
    public int MaxLevel { get; set; }
    public string Icon { get; set; }
    public bool IsUnlocked { get; set; }
    public List<Skill> Prerequisites { get; set; }

    public Skill(string name, string description)
    {
        SkillName = name;
        Description = description;
        IsUnlocked = false;
        Prerequisites = new List<Skill>();
    }

    public bool CanUnlock()
    {
        foreach (var prereq in Prerequisites)
        {
            if (!prereq.IsUnlocked)
            {
                return false;
            }
        }
        return true;
    }

    public void Unlock()
    {
        if (CanUnlock())
        {
            IsUnlocked = true;
        }
    }
}

public class SkillTree
{
    public List<Skill> Skills { get; private set; }

    public SkillTree()
    {
        Skills = new List<Skill>();
    }

    public void AddSkill(Skill skill)
    {
        Skills.Add(skill);
    }

    public Skill GetSkillByName(string name)
    {
        return Skills.Find(skill => skill.SkillName == name);
    }
}
