using UnityEngine;

public enum ItemType
{
    Goods, // ��ȭ
    Weapon,
    Armor
}

public enum Attribute
{
    Agility,
    Intellect,
    Stamina,
    Strength
}

public class ItemObject : ScriptableObject
{
    public int ID;
    public Sprite uiDisplay;
    public ItemType type;
    [TextArea(15, 20)]
    public string description;
    public ItemBuff[] buffs;

    public Item CreateItem()
    {
        Item newItem = new Item(this);
        return newItem;
    }
}

[System.Serializable]
public class Item
{
    public int Id;
    public string Name;
    public ItemBuff[] buffs;

    public Item()
    {
        Name = "";
        Id = -1;
    }

    public Item(ItemObject item)
    {
        Name = item.name;
        Id = item.ID;
        buffs = new ItemBuff[item.buffs.Length];

        for(int i = 0; i < buffs.Length; i++)
        {
            buffs[i] = new ItemBuff(item.buffs[i].min, item.buffs[i].max);
            buffs[i].attribute = item.buffs[i].attribute;
        }
    }
}
[System.Serializable]
public class ItemBuff
{
    public Attribute attribute;
    public int value;
    public int min;
    public int max;
    public ItemBuff(int _min, int _max)
    {
        min = _min;
        max = _max;
        GenerateValue();
    }
    public void GenerateValue()
    {
        value = Random.Range(min, max);
    }
}