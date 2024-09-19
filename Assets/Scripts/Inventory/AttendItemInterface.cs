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
    }

    public override void UpdateSlots()
    {
        int LoginCount = DataBaseManager.Inst.GetLoginCountMonth();
        int loginCounts = 0;

        foreach (KeyValuePair<GameObject, AttendItem> _slot in itemsDisplayed)
        {
            GameObject IconObj = _slot.Key.transform.GetChild(0).gameObject;
            GameObject BlerImg = _slot.Key.transform.GetChild(2).gameObject;
            Image iconImage = IconObj.GetComponent<Image>();

            if (GameDataManager.Inst.GoodsItemInfoList != null)
            {
                GameDataManager.Inst.GoodsItemInfoList.TryGetValue(_slot.Value.ClassName, out Goods item);

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

}
