using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CommonPanel : UIPanel
{
    [SerializeField]
    private DialogWindow _dialogWindow;
    [SerializeField]
    private OptionWindow _optionWindow;
    [SerializeField]
    private TipWindow _tipWindow;

    private void Awake()
    {
        if(_dialogWindow == null)
        {
            _dialogWindow = GetComponentInChildren<DialogWindow>();
        }
        if (_optionWindow == null)
        {
            _optionWindow = GetComponentInChildren<OptionWindow>();
        }
        if(_tipWindow == null)
        {
            _tipWindow = GetComponentInChildren<TipWindow>();
        }

        _dialogWindow.gameObject.SetActive(false);
        _optionWindow.gameObject.SetActive(false);
        //_tipWindow.gameObject.SetActive(false);
    }

    public void ShowDialogWindow(string context, float time, Action callback, Vector3 wPos, bool follow = false, GameObject followTarget = null, Vector3 offset = default(Vector3))
    {
        _dialogWindow.ShowDialogWindow(context, time, callback, wPos, follow, followTarget, offset);
    }

    public void ShowOptionWindow(Action<InteractionOption> callback)
    {
        _optionWindow.Show(callback);
    }

    public void ShowCenterTip(string context, string btnText = "关闭", Action callback = null)
    {
        _tipWindow.ShowCenterTip(context, btnText, callback);
    }

    public void ShowBottomTip(string context)
    {
        _tipWindow.ShowBottomTip(context);
    }

    public void HideBottomTip()
    {
        _tipWindow.HideBottomTip();
    }

    public override void Reset()
    {
        _dialogWindow.gameObject.SetActive(false);
        _optionWindow.gameObject.SetActive(false);
        _tipWindow.Reset();
    }
}
