using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class ItemUI : MonoBehaviour
{
    [SerializeField]
    private Text _itemText;
    private ItemInfo _itemInfo;
    public Action<int> ClickUseEvent;

    public void Init(ItemInfo itemInfo)
    {
        _itemInfo = itemInfo;
        _itemText.text = $"{itemInfo.item.name} x {itemInfo.count}";
    }

    public void OnClickUseBtn()
    {
        AudioManager.Instance.PlayAudio("Click01", false, 0.5f);
        ClickUseEvent?.Invoke(_itemInfo.item.id);
    }

    public void ShowDescriptionTip()
    {
        UIManager.Instance.GetUIPanel<CommonPanel>().ShowCenterTip(_itemInfo.item.description);
    }
}
