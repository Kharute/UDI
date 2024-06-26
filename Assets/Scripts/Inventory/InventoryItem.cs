using UnityEngine;
[CreateAssetMenu(fileName = "New Inventory Item", menuName = "Inventory/Item")]


public class InventoryItem : ScriptableObject
{
    public int itemID;
    public ItemType itemType;
    public string itemName;
    public Sprite icon;
    public string description;
    
}
[System.Serializable]
public class InventoryItemData
{
    public int itemID;
    public ItemType itemType;
    public string name;
    public string iconPath;
    public string description;
    
}