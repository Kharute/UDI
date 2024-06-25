using UnityEngine;
[CreateAssetMenu(fileName = "New Inventory Item", menuName = "Inventory/Item")]
public class InventoryItem : ScriptableObject
{
    public int itemID;
    public string itemName;
    public Sprite icon;
    public string description;
    public string rarity;
}
[System.Serializable]
public class InventoryItemData
{
    public int id;
    public string name;
    public string iconPath;
    public string description;
    public string rarity;
}