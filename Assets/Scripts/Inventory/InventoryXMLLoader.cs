using System;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class InventoryXMLLoader : MonoBehaviour
{
    public TextAsset xmlFile;

    public List<Item> LoadItems()
    {
        List<Item> items = new List<Item>();
        XmlDocument xmlDoc = new XmlDocument();

        xmlDoc.LoadXml(xmlFile.text);

        XmlNodeList itemList = xmlDoc.GetElementsByTagName("data");
        foreach (XmlNode itemNode in itemList)
        {
            Item itemData = new Item();
            itemData.ItemKey.ItemID = int.Parse(itemNode.Attributes["ItemID"].Value);
            itemData.ItemKey.ItemType = (ItemType)Enum.Parse(typeof(ItemType), itemNode.Attributes["ItemType"].Value);
            itemData.ItemName = itemNode.Attributes["ItemName"].Value;
            itemData.Icon = itemNode.Attributes["Icon"].Value;
            itemData.Description = itemNode.Attributes["Description"].Value;
            
            items.Add(itemData);
        }

        return items;
    }
}
