using System.Collections;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class SkillView : MonoBehaviour
{

    #region Movement Define

    [SerializeField] float moveValue = 250f;
    [SerializeField] float rolltime = 1.5f;
    bool isOpened = false;

    #endregion

    [SerializeField] GameObject Prefab_SkillSlot;
    [SerializeField] GameObject Prefab_VerticalLayoutGroup;
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
        var skillTreeList = GameDataManager.Inst.SkillTreeList;
        var skillInfoList = GameDataManager.Inst.SkillInfoList;
        
        if (skillTreeList != null && skillInfoList != null)
        {
            //3개면 3개를 하나로 묶어서 넣어야되고, 1개면 하나만 묶어서 넣어야 되고
            foreach (var skill in skillTreeList)
            {
                var layout1 = Instantiate(Prefab_VerticalLayoutGroup, Transform_SlotRoot.transform);
                //Prefab_VerticalLayoutGroup.GetComponent<VerticalLayoutGroup>();

                SkillTreeSlot skillSlots = skill.Value;

                if (skillSlots.SkillNames.Count > 0)
                {
                    for (int i = 0; i < skillSlots.SkillNames.Count; i++)
                    {
                        if (!string.IsNullOrEmpty(skillSlots.SkillNames[i]))
                        {
                            string sname = skillSlots.SkillNames[i];

                            var skillSlot = Prefab_SkillSlot.GetComponent<SkillSlotView>();
                            skillSlot.SetUI(sname);

                            Instantiate(skillSlot, layout1.transform);
                        }
                    }
                }
            }
        }
        else
        {
            Debug.LogError("skillTreeList, skillInfoList 중 하나가 비었음.");
        }
    }
    #endregion
}
