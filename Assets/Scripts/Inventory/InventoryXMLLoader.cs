using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class InventoryXMLLoader : MonoBehaviour
{
    public TextAsset xmlFile;

    public List<InventoryItemData> LoadItems()
    {
        List<InventoryItemData> items = new List<InventoryItemData>();
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(xmlFile.text);

        XmlNodeList itemList = xmlDoc.GetElementsByTagName("data");
        foreach (XmlNode itemNode in itemList)
        {
            InventoryItemData itemData = new InventoryItemData();
            itemData.id = int.Parse(itemNode.Attributes["ItemID"].Value);
            itemData.name = itemNode.Attributes["ItemName"].Value;
            itemData.iconPath = itemNode.Attributes["Icon"].Value;
            itemData.description = itemNode.Attributes["Description"].Value;
            itemData.rarity = itemNode.Attributes["Rarlity"].Value;
            items.Add(itemData);
        }

        return items;
    }
}
