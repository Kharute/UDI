using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class GameDataManager : MonoBehaviour
{
    public static GameDataManager Inst { get; private set; }

    // 레벨 관련, 아이템 정보,
    public Dictionary<int, Levels> LevelInfoList { get; private set; }
    public Dictionary<int, Item> ItemInfoList { get; private set; }

    private string _dataRootPath;

    private void Awake()
    {
        Inst = this;
        _dataRootPath = Application.dataPath;
        ReadAllDataOnAwake();
    }

    private void ReadAllDataOnAwake()
    {
        ReadData(nameof(Levels)); // == ReadData("Character")
        ReadData(nameof(Item));
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
        }
    }
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
        /*ItemInfoList = new Dictionary<int, Item>();

        XDocument doc = XDocument.Load($"{_dataRootPath}/{tableName}.xml");
        var dataElements = doc.Descendants("data");

        foreach (var data in dataElements)
        {
            var tempCharacter = new Character();
            tempCharacter.DataId = int.Parse(data.Attribute(nameof(tempCharacter.DataId)).Value);
            tempCharacter.Name = data.Attribute(nameof(tempCharacter.Name)).Value;
            tempCharacter.Description = data.Attribute(nameof(tempCharacter.Description)).Value;
            tempCharacter.IconPath = data.Attribute(nameof(tempCharacter.IconPath)).Value;
            tempCharacter.PrefabPath = data.Attribute(nameof(tempCharacter.PrefabPath)).Value;

            string skillNameListStr = data.Attribute("SkillNameList").Value;
            if (!string.IsNullOrEmpty(skillNameListStr))
            {
                skillNameListStr = skillNameListStr.Replace("{", string.Empty);
                skillNameListStr = skillNameListStr.Replace("}", string.Empty);

                var skillNames = skillNameListStr.Split(',');

                var list = new List<string>();
                if (skillNames.Length > 0)
                {
                    foreach (var name in skillNames)
                    {
                        list.Add(name);
                    }
                }
                tempCharacter.SkillClassNameList = list;
            }

            LoadedCharacterList.Add(tempCharacter.DataId, tempCharacter);
        }*/
    }



}
