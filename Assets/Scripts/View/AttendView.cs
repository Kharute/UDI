using System.Threading.Tasks;
using UnityEngine;

public class AttendView : MonoBehaviour
{
    int login_count;

    public void OnClick_Attend(bool isOpen)
    {
        gameObject.SetActive(isOpen);
    }

    public void OnClick_AcceptAttend()
    {
        if (!DataBaseManager.Inst.IsFirstLogin)
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
                        case "gold":
                            type = UserGoodsType.gold;
                            _ = DataBaseManager.Inst.RequestGoodsChange(type, attendItem.Amount);
                            break;
                        case "jewel":
                            type = UserGoodsType.jewel;
                            _ = DataBaseManager.Inst.RequestGoodsChange(type, attendItem.Amount);
                            break;
                        case "ticket_weapon":
                            type = UserGoodsType.ticket_weapon;
                            _ = DataBaseManager.Inst.RequestGoodsChange(type, attendItem.Amount);
                            break;
                        case "ticket_armor":
                            type = UserGoodsType.ticket_armor;
                            _ = DataBaseManager.Inst.RequestGoodsChange(type, attendItem.Amount);
                            break;
                    }
                    DataBaseManager.Inst.IsFirstLogin = !DataBaseManager.Inst.IsFirstLogin;
                }
            }
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
