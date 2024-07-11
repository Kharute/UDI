using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public class InventoryItemCreator : MonoBehaviour
{
    public InventoryXMLLoader loader;

    [ContextMenu("Create Inventory Items")]
    public void CreateInventoryItems()
    {
        /*List<Item> items = loader.LoadItems();
        foreach (Item itemData in items)
        {
            Item_Scrptable item = ScriptableObject.CreateInstance<Item_Scrptable>();
            item.itemKey.ItemID = itemData.ItemKey.ItemID;
            item.itemKey.ItemType = itemData.ItemKey.ItemType;
            item.itemName = itemData.name;
            item.icon = AssetDatabase.LoadAssetAtPath<Sprite>($"Assets/Resources/Icons/{itemData.iconPath}.png");
            item.description = itemData.description;

            string assetPath = $"Assets/InventoryItems/{item.itemKey.itemID}_{item.itemName}.asset";
            AssetDatabase.CreateAsset(item, assetPath);
        }*/

        //AssetDatabase.SaveAssets();
    }
}