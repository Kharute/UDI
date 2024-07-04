using DG.Tweening;
using System.Collections;
using UnityEngine;

public class WeaponView : MonoBehaviour
{
    #region Movement Define

    [SerializeField] float moveValue = 200f;
    [SerializeField] float rolltime = 1.5f;
    bool isOpened = false;

    #endregion

    [SerializeField] GameObject Prefab_WeaponSlot;
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
        var weaponItemList = GameDataManager.Inst.WeaponInfoList;

        if (weaponItemList != null)
        {
            foreach (var weapon in weaponItemList)
            {
                var weaponSlots = Prefab_WeaponSlot.GetComponent<WeaponSlotView>();
                weaponSlots.SetUI(weapon.Value.ItemID);

                Instantiate(Prefab_WeaponSlot, Transform_SlotRoot.transform);
            }
        }
    }
    #endregion
}
