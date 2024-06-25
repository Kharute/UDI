using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public class InventoryItemCreator : MonoBehaviour
{
    public InventoryXMLLoader loader;

    [ContextMenu("Create Inventory Items")]
    public void CreateInventoryItems()
    {
        List<InventoryItemData> items = loader.LoadItems();
        foreach (InventoryItemData itemData in items)
        {
            InventoryItem item = ScriptableObject.CreateInstance<InventoryItem>();
            item.itemID = itemData.id;
            item.itemName = itemData.name;
            item.icon = AssetDatabase.LoadAssetAtPath<Sprite>($"Assets/Resources/Icons/{itemData.iconPath}.png");
            item.description = itemData.description;
            item.rarity = itemData.rarity;

            string assetPath = $"Assets/InventoryItems/{item.itemID}_{item.itemName}.asset";
            AssetDatabase.CreateAsset(item, assetPath);
        }

        AssetDatabase.SaveAssets();
    }
}