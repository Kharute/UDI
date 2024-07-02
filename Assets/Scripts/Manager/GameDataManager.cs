using System;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public enum ItemType
{
    Weapon,
    Armor,
    Goods
}

/// <summary>
/// 인게임 내에서만 다루는 데이터를 불러오는 Manager
/// </summary>
public class GameDataManager : MonoBehaviour
{
    public static GameDataManager _instance = null;

    // 레벨 관련, 아이템 정보,
    public Dictionary<int, Levels> LevelInfoList { get; private set; }
    public Dictionary<string, Item> ItemInfoList { get; private set; }
    public Dictionary<int, AttendItem> AttendItemInfoList { get; private set; }
    public Dictionary<string, Skill> SkillInfoList { get; private set; }
    public Dictionary<int, SkillTreeSlot> SkillTreeList { get; private set; }

    private string _dataRootPath;

    public static GameDataManager Inst
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<GameDataManager>();
            }
            return _instance;
        }
    }

    private void Awake()
    {
        _dataRootPath = Application.dataPath;
        ReadAllDataOnAwake();
    }

    private void ReadAllDataOnAwake()
    {
        ReadData(nameof(Levels));
        ReadData(nameof(Item));
        ReadData(nameof(AttendItem));
        ReadData(nameof(Skill));
    }

    private void ReadData(string tableName)
    {
        switch (tableName)
        {
            case nameof(Levels):
                ReadLevelsTable(tableName);
                break;
            case nameof(Item):
                ReadItemTable(tableName);
                break;
            case nameof(AttendItem):
                ReadAttendItemTable(tableName);
                break;
            case nameof(Skill):
                ReadSkillTable(tableName);
                break;
            case nameof(SkillTreeSlot):
                ReadSkillTreeTable(tableName);
                break;
        }
    }

    #region Read XML Files

    private void ReadLevelsTable(string tableName)
    {
        LevelInfoList = new Dictionary<int, Levels>();

        XDocument doc = XDocument.Load($"{_dataRootPath}/{tableName}.xml");
        var dataElements = doc.Descendants("data");

        foreach (var data in dataElements)
        {
            var tempLevel = new Levels();
            tempLevel.LEVEL = int.Parse(data.Attribute(nameof(tempLevel.LEVEL)).Value);
            tempLevel.REQEXP = int.Parse(data.Attribute(nameof(tempLevel.REQEXP)).Value);
            tempLevel.ATK = int.Parse(data.Attribute(nameof(tempLevel.ATK)).Value);
            tempLevel.DEF = int.Parse(data.Attribute(nameof(tempLevel.DEF)).Value);
            tempLevel.SPD = int.Parse(data.Attribute(nameof(tempLevel.SPD)).Value);

            LevelInfoList.Add(tempLevel.LEVEL, tempLevel);
        }
    }

    private void ReadItemTable(string tableName)
    {
        ItemInfoList = new Dictionary<string, Item>();

        XDocument doc = XDocument.Load($"{_dataRootPath}/{tableName}.xml");
        var dataElements = doc.Descendants("data");

        foreach (var data in dataElements)
        {
            Item itemData = new Item();

            itemData.ClassName = data.Attribute(nameof(itemData.ClassName)).Value;
            itemData.ItemID = int.Parse(data.Attribute(nameof(itemData.ItemID)).Value);
            itemData.ItemType = (ItemType)Enum.Parse(typeof(ItemType), data.Attribute(nameof(itemData.ItemType)).Value);
            itemData.ItemName = data.Attribute(nameof(itemData.ItemName)).Value;
            itemData.Icon = data.Attribute(nameof(itemData.Icon)).Value;
            itemData.Description = data.Attribute(nameof(itemData.Description)).Value;

            ItemInfoList.Add(itemData.ClassName, itemData);
        }
    }

    private void ReadAttendItemTable(string tableName)
    {
        AttendItemInfoList = new Dictionary<int, AttendItem>();

        XDocument doc = XDocument.Load($"{_dataRootPath}/{tableName}.xml");
        var dataElements = doc.Descendants("data");

        foreach (var data in dataElements)
        {
            AttendItem attendItemData = new AttendItem();
            attendItemData.Day = int.Parse(data.Attribute(nameof(attendItemData.Day)).Value);
            attendItemData.ClassName = data.Attribute(nameof(attendItemData.ClassName)).Value;
            attendItemData.Amount = int.Parse(data.Attribute(nameof(attendItemData.Amount)).Value);

            AttendItemInfoList.Add(attendItemData.Day, attendItemData);
        }
    }

    private void ReadSkillTable(string tableName)
    {
        SkillInfoList = new Dictionary<string, Skill>();

        XDocument doc = XDocument.Load($"{_dataRootPath}/{tableName}.xml");
        var dataElements = doc.Descendants("data");

        foreach (var data in dataElements)
        {
            Skill skill;

            string name = data.Attribute(nameof(skill.SkillName)).Value;
            string description = data.Attribute(nameof(skill.Description)).Value;
            skill = new Skill(name, description);

            SkillType value = (SkillType)Enum.Parse(typeof(SkillType), data.Attribute(nameof(skill.Type)).Value);
            skill.Type = value;
            skill.Value = int.Parse(data.Attribute(nameof(skill.Value)).Value);
            skill.MaxLevel = int.Parse(data.Attribute(nameof(skill.MaxLevel)).Value);
            skill.Icon = data.Attribute(nameof(skill.Icon)).Value;

            string skillListStr = data.Attribute(nameof(skill.Prerequisites)).Value;
            if (!string.IsNullOrEmpty(skillListStr))
            {
                skillListStr = skillListStr.Replace("{", string.Empty);
                skillListStr = skillListStr.Replace("}", string.Empty);

                var names = skillListStr.Split(',');

                if (names.Length > 0)
                {
                    foreach (var skillname in names)
                    {
                        skill.Prerequisites.Add(new Skill(skillname, ""));
                    }
                }
            }
            SkillInfoList.Add(skill.SkillName, skill);
        }

    }
    private void ReadSkillTreeTable(string tableName)
    {
        SkillTreeList = new Dictionary<int, SkillTreeSlot>();

        XDocument doc = XDocument.Load($"{_dataRootPath}/{tableName}.xml");
        var dataElements = doc.Descendants("data");

        foreach (var data in dataElements)
        {
            SkillTreeSlot skillTreeData = new SkillTreeSlot();

            skillTreeData.SkillTreeLevel = int.Parse(data.Attribute(nameof(skillTreeData.SkillTreeLevel)).Value);
            skillTreeData.minUnlockCount = int.Parse(data.Attribute(nameof(skillTreeData.minUnlockCount)).Value);
            skillTreeData.SkillNames[0] = data.Attribute($"Slot_1").Value;
            skillTreeData.SkillNames[1] = data.Attribute($"Slot_2").Value;
            skillTreeData.SkillNames[2] = data.Attribute($"Slot_3").Value;

            SkillTreeList.Add(skillTreeData.SkillTreeLevel, skillTreeData);
        }
    }
    #endregion

}
