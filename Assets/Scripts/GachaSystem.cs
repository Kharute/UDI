using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class GachaSystem : MonoBehaviour
{
    public TextMeshProUGUI resultText;

    public void OnGachaButtonPressed(bool is100)
    {
        //t = 10, f = 100
        if(is100)
        {
            _ = DataBaseManager.Inst.RequestGacha(10, DisplayGachaResult);
        }
        else
        {
            _ = DataBaseManager.Inst.RequestGacha(100, DisplayGachaResult);
        }
    }

    private void DisplayGachaResult(Dictionary<int, int> response)
    {
        resultText.text = "Gacha Result:\n";
        foreach (var item in response)
        {
            resultText.text += $"Weapon ID: {item.Key}, Count: {item.Value}\n";
        }
        Debug.Log(resultText.text);
    }

    public void OnClick_OpenClose(bool open)
    {
        gameObject.SetActive(open);
    }

}