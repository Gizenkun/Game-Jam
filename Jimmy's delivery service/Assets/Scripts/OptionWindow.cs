using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class OptionWindow : MonoBehaviour
{
    private Action<InteractionOption> _callback;
    public void Show(Action<InteractionOption> callback)
    {
        gameObject.SetActive(true);
        _callback = callback;
    }

    public void OnClickOption(string option)
    {
        AudioManager.Instance.PlayAudio("Click01", false, 0.5f);
        foreach (InteractionOption item in Enum.GetValues(typeof(InteractionOption)))
        {
            Debug.Log(item.ToString());
            if(item.ToString() == option)
            {
                _callback?.Invoke(item);
                _callback = null;
            }
        }
        gameObject.SetActive(false);
    }
}
