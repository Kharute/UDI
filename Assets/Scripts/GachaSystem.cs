using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GachaSystem : MonoBehaviour
{
    public TextMeshProUGUI resultText;
    public void OnGacha10ButtonPressed()
    {
        DataBaseManager.Inst.RequestGacha(10, DisplayGachaResult);
    }

    public void OnGacha100ButtonPressed()
    {
        DataBaseManager.Inst.RequestGacha(100, DisplayGachaResult);
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