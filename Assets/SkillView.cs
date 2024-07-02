using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;
using ViewModel.Extentions;

public class SkillView : MonoBehaviour
{

    #region Movement Define

    [SerializeField] float moveValue = 200f;
    [SerializeField] float rolltime = 1.5f;
    bool isOpened = false;

    #endregion
    [SerializeField] GameObject Prefab_SkillSlot;
    [SerializeField] GameObject Transform_SlotRoot;


    private void Start()
    {
        SetSkillUI();
    }

    #region Movement Method
    public void OnClick_UI()
    {
        StartCoroutine(SlideStateChange());
    }

    IEnumerator SlideStateChange()
    {
        isOpened = !isOpened;

        float upValue = isOpened ? 1 : -1;
        Vector3 vec = transform.position;
        transform.DOMove(vec + Vector3.up * moveValue * upValue, rolltime);

        yield return new WaitForSeconds(rolltime);
    }
    #endregion


    #region Method

    private void SetSkillUI()
    {
        // ��ųƮ������ 
        var skillTreeList = GameDataManager.Inst.SkillTreeList;
        var skillInfoList = GameDataManager.Inst.SkillInfoList;
        
        if (skillTreeList.Count > 0 && skillInfoList.Count > 0)
        {
            //3���� 3���� �ϳ��� ��� �־�ߵǰ�, 1���� �ϳ��� ��� �־�� �ǰ�
            foreach ( var skill in skillTreeList )
            {
                var gObj = Instantiate(new GameObject(), Transform_SlotRoot.transform);
                var layoutGroup = gObj.AddComponent<VerticalLayoutGroup>();

                SkillTreeSlot skillSlot = skill.Value;

                for (int i = 0; i < 3; i++)
                {
                    if(!string.IsNullOrEmpty(skillSlot.SkillNames[i]))
                    {
                        string sname = skillSlot.SkillNames[i];
                        skillInfoList[sname].Value;// �̰� ��ų �̸���.
                    }
                    

                }
                Instantiate(Prefab_SkillSlot, layoutGroup.transform);
                if (skillSlot == null)
                    continue;

            }
            skillTreeList.Values.

            for (var skillClassName in skillNameList)
            {

                var gObj = Instantiate(Prefab_SkillSlot, Transform_SlotRoot.transform);
                var skillSlot = gObj.GetComponent<SkillSlotView>();
                if (skillSlot == null)
                    continue;

                skillSlot.SetUI(skillClassName.Key);
            }
        }
    }
    #endregion





}
