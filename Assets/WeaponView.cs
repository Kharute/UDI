using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponView : MonoBehaviour
{
    #region Movement Define

    [SerializeField] float moveValue = 200f;
    [SerializeField] float rolltime = 1.5f;
    bool isOpened = false;

    #endregion

    [SerializeField] GameObject Prefab_SkillSlot;
    [SerializeField] GameObject Prefab_HorizontalLayoutGroup;
    [SerializeField] GameObject Transform_SlotRoot;


    private void Start()
    {
        SetSkillUI();
    }

    #region Movement Method
    public void OnClick_UI()
    {
        isOpened = !isOpened;
        if (isOpened)
        {
            gameObject.SetActive(true);
        }

        StartCoroutine(SlideStateChange());
    }

    IEnumerator SlideStateChange()
    {
        float upValue = isOpened ? 1 : -1;
        Vector3 vec = transform.position;
        transform.DOMove(vec + Vector3.up * moveValue * upValue, rolltime);
        yield return new WaitForSeconds(rolltime);

        if (!isOpened)
        {
            gameObject.SetActive(false);
        }
    }
    #endregion


    #region Method

    private void SetSkillUI()
    {
        var weaponItemList = GameDataManager.Inst.WeaponItemList;
        var weaponSlotList = GameDataManager.Inst.WeaponSlotList;

        if (weaponItemList != null && weaponSlotList != null)
        {
            foreach (var weapon in weaponSlotList)
            {
                var weaponSlot = Prefab_SkillSlot.GetComponent<WeaponSlotView>();
                weaponSlot.SetUI(weapon.Key);

                Instantiate(weaponSlot, Transform_SlotRoot.transform);
            }
        }
        /*else
        {
            Debug.LogError("skillTreeList, skillInfoList 중 하나가 비었음.");
        }*/
    }
    #endregion
}
