using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item Database", menuName = "Inventory System/Items/Database")]
public class ItemDatabaseObject : ScriptableObject, ISerializationCallbackReceiver
{
    public InventoryItem[] items;
    //public Dictionary<ItemObject, int> GetId = new Dictionary<ItemObject, int>();
    public Dictionary<int, InventoryItem> GetItem = new Dictionary<int, InventoryItem>();

    public void OnAfterDeserialize()
    {
        /*GetId = new Dictionary<ItemObject, int>(GetId);
        GetItem = new Dictionary<int, ItemObject>(GetItem);*/
        /*if (items != null)
        {
            for (int i = 0; i < items.Length; i++)
            {
                items[i].itemID = i;
                GetItem.Add(i, items[i]);
            }
        }*/
    }

    public void OnBeforeSerialize()
    {

    }


}
