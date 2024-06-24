using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerDetailView : MonoBehaviour
{
    [SerializeField]
    int level = 0;

    [SerializeField] TextMeshProUGUI TextMesh_Level;
    [SerializeField] TextMeshProUGUI TextMesh_Atk;
    [SerializeField] TextMeshProUGUI TextMesh_Def;
    [SerializeField] TextMeshProUGUI TextMesh_Spd;
    [SerializeField] TextMeshProUGUI TextMesh_ReqExp;

    private void OnEnable()
    {
        level = DataBaseManager.Inst.GetPlayerLevel();

        if (level != 0)
            SetPlayerDetailInfo(level);
    }

    public void OnButtonClick_PlayerDetailInfo(bool isOpen)
    {
        gameObject.SetActive(isOpen);
    }

    public void SetPlayerDetailInfo(int level)
    {
        if(level > 0)
        {
            var character = GameDataManager.Inst.GetPlayerDetailData(level);
            if (character == null)
            {
                return;
            }
             TextMesh_Level.text = character.LEVEL.ToString();
             TextMesh_Atk.text = character.ATK.ToString();
             TextMesh_Def.text = character.DEF.ToString();
             TextMesh_Spd.text = character.SPD.ToString();
            TextMesh_ReqExp.text = character.REQEXP.ToString();
            
        }
        
    }
}
