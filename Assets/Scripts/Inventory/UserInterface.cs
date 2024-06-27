using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public abstract class UserInterface : MonoBehaviour
{
    //public Player player;

    public Dictionary<GameObject, AttendItem> itemsDisplayed = new Dictionary<GameObject, AttendItem>();
    void Start()
    {
        CreateSlots();
        
        /*AddEvent(gameObject, EventTriggerType.PointerEnter, delegate { OnEnterInterface(gameObject); });
        AddEvent(gameObject, EventTriggerType.PointerExit, delegate { OnExitInterface(gameObject); });*/
    }

    private void OnEnable()
    {
        UpdateSlots();
    }
    public abstract void CreateSlots();

    /*void Update()
    {
        UpdateSlots();
    }*/

    void UpdateSlots()
    {
        int LoginCount = DataBaseManager.Inst.GetLoginCountMonth();
        int loginCounts = 0;

        foreach (KeyValuePair<GameObject, AttendItem> _slot in itemsDisplayed)
        {
            GameObject IconObj = _slot.Key.transform.GetChild(0).gameObject;
            GameObject BlerImg = _slot.Key.transform.GetChild(2).gameObject;
            Image iconImage = IconObj.GetComponent<Image>();

            
            ItemKey itemKey = new ItemKey();
            itemKey.SetItemKey(_slot.Value.ItemID, ItemType.Goods);

            if (GameDataManager.Inst.ItemInfoList.ContainsKey(itemKey))
            {
                GameDataManager.Inst.ItemInfoList.TryGetValue(itemKey, out Item item);
                
                var path = $"Icons/{item.Icon}";
                iconImage.sprite = Resources.Load<Sprite>(path);
                
                iconImage.color = new Color(1, 1, 1, 1);
                _slot.Key.GetComponentInChildren<TextMeshProUGUI>().text = _slot.Value.Amount == 1 ? "" : _slot.Value.Amount.ToString("n0");

                if (loginCounts++ < LoginCount)
                    BlerImg.SetActive(true);
                else
                    BlerImg.SetActive(false);
            }
        }
    }

    protected void AddEvent(GameObject obj, EventTriggerType type, UnityAction<BaseEventData> action)
    {
        EventTrigger trigger = obj.GetComponent<EventTrigger>();
        var eventTrigger = new EventTrigger.Entry();
        eventTrigger.eventID = type;
        eventTrigger.callback.AddListener(action);
        trigger.triggers.Add(eventTrigger);
    }

    /*public void OnEnter(GameObject obj)
    {
        player.mouseItem.hoverObj = obj;
        if (itemsDisplayed.ContainsKey(obj))
            player.mouseItem.hoverItem = itemsDisplayed[obj];
    }
    public void OnExit(GameObject obj)
    {
        player.mouseItem.hoverObj = null;
        player.mouseItem.hoverItem = null;
    }
    public void OnEnterInterface(GameObject obj)
    {
        player.mouseItem.ui = obj.GetComponent<UserInterface>();
    }
    public void OnExitInterface(GameObject obj)
    {
        player.mouseItem.ui = null;
    }
    
    public void OnDrag(GameObject obj)
    {
        if (player.mouseItem.obj != null)
            player.mouseItem.obj.GetComponent<RectTransform>().position = Input.mousePosition;
    }*/


}
public class MouseItem
{
    public UserInterface ui;
    public GameObject obj;
    public InventorySlot item;
    public InventorySlot hoverItem;
    public GameObject hoverObj;
}