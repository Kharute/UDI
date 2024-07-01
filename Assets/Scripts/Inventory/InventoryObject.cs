using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEditor;
using System.Runtime.Serialization;

[CreateAssetMenu(fileName = "New Inventory", menuName = "Inventory System/Inventory")]
public class InventoryObject : ScriptableObject
{
    public string savePath;
    public ItemDatabaseObject Attend_DataBase;
    public Inventory Container;

    public void AddItem(Item_Scrptable _item, int _amount)
    {
        for (int i = 0; i < Container.Items.Length; i++)
        {
            if (Container.Items[i].item.ItemID == _item.ItemID)
            {
                Container.Items[i].AddAmount(_amount);
                return;
            }
        }
        SetEmptySlot(_item, _amount);

    }
    public InventorySlot SetEmptySlot(Item_Scrptable _item, int _amount)
    {
        for (int i = 0; i < Container.Items.Length; i++)
        {
            if (Container.Items[i].item.ItemID <= 0)
            {
                Container.Items[i].UpdateSlot(_item, _amount);
                return Container.Items[i];
            }
        }
        //set up functionality for full inventory
        return null;
    }

    public void MoveItem(InventorySlot item1, InventorySlot item2)
    {
        InventorySlot temp = new InventorySlot(item2.item, item2.amount);
        item2.UpdateSlot(item1.item, item1.amount);
        item1.UpdateSlot(temp.item, temp.amount);
    }

    public void RemoveItem(Item_Scrptable _item)
    {
        for (int i = 0; i < Container.Items.Length; i++)
        {
            if (Container.Items[i].item == _item)
            {
                Container.Items[i].UpdateSlot(null, 0);
            }
        }
    }

    [ContextMenu("Save")]
    public void Save()
    {
        string saveData = JsonUtility.ToJson(this, true);
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(string.Concat(Application.dataPath, savePath));
        bf.Serialize(file, saveData);
        file.Close();

        /*IFormatter formatter = new BinaryFormatter();
        Stream stream = new FileStream(string.Concat(Application.persistentDataPath, savePath), FileMode.Create, FileAccess.Write);
        formatter.Serialize(stream, Container);
        stream.Close();*/
    }

    [ContextMenu("Load")]
    public void Load()
    {
        string fullPath = string.Concat(Application.dataPath, savePath);
        Debug.Log("Loading from path: " + fullPath);

        if (File.Exists(fullPath))
        {
            Debug.Log("File exists. Attempting to load.");
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(fullPath, FileMode.Open);

            try
            {
                JsonUtility.FromJsonOverwrite(bf.Deserialize(file).ToString(), this);
                Debug.Log("Load successful.");
            }
            catch (SerializationException e)
            {
                Debug.LogError("Failed to load file. Error: " + e.Message);
            }
            finally
            {
                file.Close();
            }
        }
        else
        {
            Debug.LogWarning("File does not exist: " + fullPath);
        }
    }

    [ContextMenu("Clear")]
    public void Clear()
    {
        Container.Clear();
    }
}
[System.Serializable]
public class Inventory
{
    public InventorySlot[] Items = new InventorySlot[31];
    public void Clear()
    {
        for (int i = 0; i < Items.Length; i++)
        {
            Items[i].UpdateSlot(new Item_Scrptable(), 0);
        }
    }
}
[System.Serializable]
public class InventorySlot
{
    public UserInterface parent;
    public int SlotID;
    public Item_Scrptable item;
    public int amount;

    public InventorySlot()
    {
        item = null;
        amount = 0;
    }
    public InventorySlot(Item_Scrptable _item, int _amount)
    {
        item = _item;
        amount = _amount;
    }
    public void UpdateSlot(Item_Scrptable _item, int _amount)
    {
        item = _item;
        amount = _amount;
    }
    public void AddAmount(int value)
    {
        amount += value;
    }
}