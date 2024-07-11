using System;
using System.Collections.Generic;
using System.Xml.Linq;
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
    public Dictionary<int, Weapon> WeaponList { get; private set; }
    public Dictionary<string, Goods> GoodsItemInfoList { get; private set; }
    public Dictionary<int, AttendItem> AttendItemInfoList { get; private set; }
    public Dictionary<string, Skill> SkillInfoList { get; private set; }
    public Dictionary<int, SkillTreeSlot> SkillTreeList { get; private set; }
    public Dictionary<int, WeaponInfo> WeaponInfoList { get; private set; }

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
        ReadData(nameof(SkillTreeSlot));
        ReadData(nameof(WeaponInfo));
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
            case nameof(WeaponInfo):
                ReadWeaponInfoTable(tableName);
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
        WeaponList = new Dictionary<int, Weapon>();
        //ItemInfoList = new Dictionary<string, Item>();
        GoodsItemInfoList = new Dictionary<string, Goods>();

        XDocument doc = XDocument.Load($"{_dataRootPath}/{tableName}.xml");
        var dataElements = doc.Descendants("data");

        foreach (var data in dataElements)
        {
            Item itemData = new Item();
            //itemData.ClassName = data.Attribute(nameof(itemData.ClassName)).Value;
            ItemType itemType = (ItemType)Enum.Parse(typeof(ItemType), data.Attribute(nameof(itemData.ItemType)).Value);

            itemData.Icon = data.Attribute(nameof(itemData.Icon)).Value;
            itemData.Description = data.Attribute(nameof(itemData.Description)).Value;

            switch(itemType)
            {
                case ItemType.Weapon:
                    Weapon weaponData = new Weapon();
                    //weaponData.ClassName = data.Attribute(nameof(weaponData.ClassName)).Value;
                    weaponData.ItemID = int.Parse(data.Attribute(nameof(weaponData.ItemID)).Value);
                    weaponData.ItemName = data.Attribute(nameof(weaponData.ItemName)).Value;
                    weaponData.Icon = data.Attribute(nameof(weaponData.Icon)).Value;
                    weaponData.Rarity = data.Attribute(nameof(weaponData.Rarity)).Value;
                    weaponData.Description = data.Attribute(nameof(weaponData.Description)).Value;
                    WeaponList.Add(weaponData.ItemID, weaponData);
                    break;
                case ItemType.Armor:
                    //ItemInfoList.Add(itemData.ClassName, itemData);
                    break;
                case ItemType.Goods:
                    Goods goodsData = new Goods();
                    goodsData.ClassName = data.Attribute(nameof(goodsData.ClassName)).Value;
                    goodsData.ItemID = int.Parse(data.Attribute(nameof(goodsData.ItemID)).Value);
                    goodsData.ItemName = data.Attribute(nameof(goodsData.ItemName)).Value;
                    goodsData.Icon = data.Attribute(nameof(goodsData.Icon)).Value;
                    goodsData.Description = data.Attribute(nameof(goodsData.Description)).Value;
                    GoodsItemInfoList.Add(goodsData.ClassName, goodsData);
                    break;
            }
            
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
            skillTreeData.MinUnlockCount = int.Parse(data.Attribute(nameof(skillTreeData.MinUnlockCount)).Value);

            for(int i = 0; i < 3; i++)
            {
                string inf = $"Slot_{i+1}";
                if (!string.IsNullOrEmpty(data.Attribute(inf).Value))
                {
                    skillTreeData.SkillNames.Add(data.Attribute(inf).Value);
                }
            }

            SkillTreeList.Add(skillTreeData.SkillTreeLevel, skillTreeData);
        }
    }

    private void ReadWeaponInfoTable(string tableName)
    {
        WeaponInfoList = new Dictionary<int, WeaponInfo>();

        XDocument doc = XDocument.Load($"{_dataRootPath}/{tableName}.xml");
        var dataElements = doc.Descendants("data");

        foreach (var data in dataElements)
        {
            WeaponInfo weaponInfoData = new WeaponInfo();

            weaponInfoData.WeaponID = int.Parse(data.Attribute(nameof(weaponInfoData.WeaponID)).Value);
            weaponInfoData.ItemID = int.Parse(data.Attribute(nameof(weaponInfoData.ItemID)).Value);
            weaponInfoData.WeaponName = data.Attribute(nameof(weaponInfoData.WeaponName)).Value;
            weaponInfoData.Tier = int.Parse(data.Attribute(nameof(weaponInfoData.Tier)).Value);

            WeaponInfoList.Add(weaponInfoData.WeaponID, weaponInfoData);
        }
    }
    #endregion

}
