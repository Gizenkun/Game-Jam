using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BackpackPanel : UIPanel
{
    [SerializeField]
    private GameObject _itemUiRoot;
    [SerializeField]
    private GameObject _itemUiPrefab;

    private List<ItemInfo> _itemList;
    private Action<int> _callback;
    private Action _cacelCallback;

    public override void Start()
    {
        base.Start();
        this.gameObject.SetActive(false);
    }

    private void Refresh()
    {
        for (int i = 0; i < _itemList.Count; i++)
        {
            ItemUI itemUI = Instantiate(_itemUiPrefab, _itemUiRoot.transform).GetComponent<ItemUI>();
            itemUI.Init(_itemList[i]);
            itemUI.ClickUseEvent = _callback;
        }
    }

    public void Show(List<ItemInfo> itemList, Action<int> callback, Action cancelCallback)
    {
        AudioManager.Instance.PlayAudio("OnePoint01", false, 1f);
        _itemList = itemList;
        _callback = (id) =>
        {
            gameObject.SetActive(false);
            callback?.Invoke(id);
        };
        _cacelCallback = cancelCallback;
        Refresh();
        gameObject.SetActive(true);
    }

    public void OnClickCancelBtn()
    {
        AudioManager.Instance.PlayAudio("Click01", false, 0.5f);
        _cacelCallback?.Invoke();
        gameObject.SetActive(false);
    }

    public void UpdateData(List<ItemInfo> itemList)
    {
        _itemList = itemList;
        Refresh();
    }

    public override void Reset()
    {
        this.gameObject.SetActive(false);
    }
}
