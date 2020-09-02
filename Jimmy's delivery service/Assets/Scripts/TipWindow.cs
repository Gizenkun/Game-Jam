using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class TipWindow : MonoBehaviour
{
    [SerializeField]
    private GameObject _centerTip;
    [SerializeField]
    private GameObject _bottomTip;
    [SerializeField]
    private Text _text;
    [SerializeField]
    private Text _btnText;
    [SerializeField]
    private Text _bottomTipText;


    private Action _callback;
    public void ShowCenterTip(string context, string btnText = "关闭", Action callback = null)
    {
        AudioManager.Instance.PlayAudio("OnePoint01", false, 1f);
        _text.text = context;
        _btnText.text = btnText;
        _centerTip.SetActive(true);
        _callback = callback;
    }

    public void ShowBottomTip(string context)
    {
        _bottomTip.SetActive(true);
        _bottomTipText.text = context;
    }

    public void HideBottomTip()
    {
        _bottomTip.SetActive(false);
    }

    public void OnClickBtn()
    {
        AudioManager.Instance.PlayAudio("Click01", false, 0.5f);
        _callback?.Invoke();
        _centerTip.SetActive(false);
    }

    public void Reset()
    {
        _centerTip.SetActive(false);
        _bottomTip.SetActive(false);
    }
}
