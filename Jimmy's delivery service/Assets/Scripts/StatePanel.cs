using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatePanel : UIPanel
{
    [SerializeField]
    private Text _moneyText;
    [SerializeField]
    private Text _satisfactionText;
    [SerializeField]
    private Text _performanceText;
    [SerializeField]
    private Text _parcelCountText;

    public void UpdateStateUI(PlayerData playerData, int parcelCount)
    {
        _moneyText.text = playerData.money.ToString();
        _satisfactionText.text = playerData.satisfaction.ToString() + "%";
        _performanceText.text = playerData.performance.ToString() + "%";
        _parcelCountText.text = parcelCount.ToString();
    }
}
