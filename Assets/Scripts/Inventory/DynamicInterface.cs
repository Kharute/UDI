using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DynamicInterface : UserInterface
{
    public GameObject inventoryPrefab;

    GridLayoutGroup gridLayoutGroup;

    public int NUMBER_OF_COLUMN;

    private void Awake()
    {
        gridLayoutGroup = GetComponent<GridLayoutGroup>();
    }
    public override void CreateSlots()
    {
        itemsDisplayed = new Dictionary<GameObject, InventorySlot>();

        //[TODO]_slot.Value.ID 뺄 방법 찾아보기
        for (int i = 0; i < inventory.Container.Items.Length; i++)
        {
            var obj = Instantiate(inventoryPrefab, gridLayoutGroup.transform.position, Quaternion.identity, transform);
            itemsDisplayed.Add(obj, inventory.Container.Items[i]);

            /*AddEvent(obj, EventTriggerType.PointerEnter, delegate { OnEnter(obj); });
            AddEvent(obj, EventTriggerType.PointerExit, delegate { OnExit(obj); });
            AddEvent(obj, EventTriggerType.BeginDrag, delegate { OnDragStart(obj); });
            AddEvent(obj, EventTriggerType.EndDrag, delegate { OnDragEnd(obj); });
            AddEvent(obj, EventTriggerType.Drag, delegate { OnDrag(obj); });*/
        }
    }
}
