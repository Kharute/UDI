using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttendView : MonoBehaviour
{
    int login_count;
    bool isAccept = false;
    DataBaseManager DBM;

    private void Awake()
    {
        DBM = DataBaseManager.Inst;
    }

    public void OnClick_AcceptAttend()
    {
        login_count = DBM.GetLoginCountMonth();
        AttendItem attendItem = GameDataManager.Inst.AttendItemInfoList[login_count];

        if (attendItem != null)
        {
            DataBaseManager.Inst.OnClick_UpdateUserGoods(attendItem.ClassName, attendItem.Amount);


        }

        
        //login_count-1ø°º≠ login_count¿ª ∫Ø∞Ê«ÿ¡‡æﬂµ .
    }
}
