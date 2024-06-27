using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item Database", menuName = "Inventory System/Items/Database")]
public class ItemDatabaseObject : ScriptableObject, ISerializationCallbackReceiver
{
    public Item_Scrptable[] items;
    //public Dictionary<ItemObject, int> GetId = new Dictionary<ItemObject, int>();
    public Dictionary<int, Item_Scrptable> GetItem = new Dictionary<int, Item_Scrptable>();

    public void OnAfterDeserialize()
    {
        /*if (items != null)
        { 
            for (int i = 0; i < items.Length; i++)
            {
                items[i].itemKey.itemID = i;
                GetItem.Add(i, items[i]);
            }
        }*/
    }

    public void OnBeforeSerialize()
    {

    }


}
