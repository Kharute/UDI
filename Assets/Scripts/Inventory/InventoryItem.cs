using UnityEngine;
using static UnityEditor.Progress;
[CreateAssetMenu(fileName = "New Inventory Item", menuName = "Inventory/Item")]

public class Item_Scrptable : ScriptableObject
{
    public ItemKey itemKey;
    public string itemName;
    public Sprite icon;
    public string description;
    
}
[System.Serializable]
public class Item
{
    public ItemKey ItemKey;
    public string ItemName;
    public string Icon;
    public string Description;   
}

public class ItemKey
{
    public ItemType ItemType;
    public int ItemID;

    public override int GetHashCode()
    {
        return ItemID;
    }

    public override bool Equals(object obj)
    {
        return Equals(obj as ItemKey);
    }

    public bool Equals(ItemKey obj)
    {
        return obj != null && obj.ItemID == this.ItemID && obj.ItemType == this.ItemType;
    }

    public void SetItemKey(int _ID, ItemType _type)
    {
        ItemID = _ID;
        ItemType = _type;
    }
}

public class AttendItem
{
    public int Day;
    public int ItemID;
    public int Amount;
}

