using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AttendItemInterface : UserInterface
{
    public GameObject inventoryPrefab;
    GridLayoutGroup gridLayoutGroup;

    public int NUMBER_OF_COLUMN;

    Dictionary<int, AttendItem> AttendItemInfoList;


    private void Awake()
    {
        gridLayoutGroup = GetComponent<GridLayoutGroup>();
        AttendItemInfoList = GameDataManager.Inst.AttendItemInfoList;
    }

    public override void CreateSlots()
    {
        foreach(var AttendItem in AttendItemInfoList)
        {
            var obj = Instantiate(inventoryPrefab, gridLayoutGroup.transform.position, Quaternion.identity, transform);
            itemsDisplayed.Add(obj, AttendItem.Value);
        }

        /*for (int i = 0; i < inventory.Container.Items.Length; i++)
        {
            var obj = Instantiate(inventoryPrefab, gridLayoutGroup.transform.position, Quaternion.identity, transform);
            itemsDisplayed.Add(obj, inventory.Container.Items[i]);

            *//*AddEvent(obj, EventTriggerType.PointerEnter, delegate { OnEnter(obj); });
            AddEvent(obj, EventTriggerType.PointerExit, delegate { OnExit(obj); });
            AddEvent(obj, EventTriggerType.BeginDrag, delegate { OnDragStart(obj); });
            AddEvent(obj, EventTriggerType.EndDrag, delegate { OnDragEnd(obj); });
            AddEvent(obj, EventTriggerType.Drag, delegate { OnDrag(obj); });*//*
        }*/
    }
    public void AttendSlot()
    {

    }

    
}
