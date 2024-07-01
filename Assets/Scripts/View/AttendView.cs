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

    public void OnClick_Attend(bool isOpen)
    {
        gameObject.SetActive(isOpen);
    }

    public void OnClick_AcceptAttend()
    {
        // ÇØ´ç UI¸¦ ²¨Áà¾ß ÇÔ.
        if (!isAccept)
        {
            login_count = DBM.GetLoginCountMonth();
            if (login_count > 0)
            {
                AttendItem attendItem = GameDataManager.Inst.AttendItemInfoList[login_count];

                UserGoodsType type;
                if (attendItem != null)
                {
                    switch (attendItem.ClassName)
                    {
                        case "GOLD":
                            type = UserGoodsType.GOLD;
                            DataBaseManager.Inst.RequestGoodsChange(type, attendItem.Amount);
                            break;
                        case "JEWEL":
                            type = UserGoodsType.JEWEL;
                            DataBaseManager.Inst.RequestGoodsChange(type, attendItem.Amount);
                            break;
                        case "TICKET_WEAPON":
                            type = UserGoodsType.TICKET_WEAPON;
                            DataBaseManager.Inst.RequestGoodsChange(type, attendItem.Amount);
                            break;
                        case "TICKET_ARMOR":
                            type = UserGoodsType.TICKET_ARMOR;
                            DataBaseManager.Inst.RequestGoodsChange(type, attendItem.Amount);
                            break;
                    }
                }
            }
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
