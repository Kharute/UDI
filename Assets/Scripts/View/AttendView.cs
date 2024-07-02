using UnityEngine;

public class AttendView : MonoBehaviour
{
    int login_count;
    bool isAccept = false;

    public void OnClick_Attend(bool isOpen)
    {
        gameObject.SetActive(isOpen);
    }

    public void OnClick_AcceptAttend()
    {
        if (!isAccept)
        {
            login_count = DataBaseManager.Inst.GetLoginCountMonth();
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
                    isAccept = !isAccept;
                }
            }
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
