using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class Cutscene : MonoBehaviour
{
    public static Cutscene Instance;

    [SerializeField]
    private Text _timeText;

    private Animator _animator;
    private Action _callback;

    private bool _isDate = false;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        _animator = GetComponent<Animator>();
        _animator.enabled = false;
    }

    private void Update()
    {
        if(_isDate)
        {
            _timeText.text = $"Day {GameManager.Instance.CurrentDay}";
        }
        else
        {
            TimeData timeData = GameManager.Instance.CurrentTime;
            _timeText.text = $"{timeData.hours} : {timeData.minutes}";
        }
    }

    public void PlayCutscene(Action callback)
    {
        _callback = callback;
        _animator.enabled = true;
        _animator.Rebind();
        _animator.Play("Cutscene");
    }

    public void SwitchToDate()
    {
        _isDate = true;
    }

    public void SwitchToTime()
    {
        _isDate = false;
    }

    public void CutsceneOver()
    {
        _animator.enabled = false;
        _callback?.Invoke();
    }
}
