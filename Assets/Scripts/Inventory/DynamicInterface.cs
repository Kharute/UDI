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

    public int X_START;
    public int Y_START;
    public int SPACE_BETWEEN_ITEM;
    public int NUMBER_OF_COLUMN;

    private void Awake()
    {
        gridLayoutGroup = GetComponent<GridLayoutGroup>();
    }
    public override void CreateSlots()
    {
        itemsDisplayed = new Dictionary<GameObject, InventorySlot>();
        for (int i = 0; i < inventory.Container.Items.Length; i++)
        {
            var obj = Instantiate(inventoryPrefab, gridLayoutGroup.transform.position, Quaternion.identity, transform);

            /*AddEvent(obj, EventTriggerType.PointerEnter, delegate { OnEnter(obj); });
            AddEvent(obj, EventTriggerType.PointerExit, delegate { OnExit(obj); });
            AddEvent(obj, EventTriggerType.BeginDrag, delegate { OnDragStart(obj); });
            AddEvent(obj, EventTriggerType.EndDrag, delegate { OnDragEnd(obj); });
            AddEvent(obj, EventTriggerType.Drag, delegate { OnDrag(obj); });*/

            itemsDisplayed.Add(obj, inventory.Container.Items[i]);
        }
    }
    public Vector3 GetPosition(int i)
    {
        return new Vector3(X_START + (SPACE_BETWEEN_ITEM * (i % NUMBER_OF_COLUMN)), -Y_START + ((-SPACE_BETWEEN_ITEM * (i / NUMBER_OF_COLUMN))), 0f);
    }
}
