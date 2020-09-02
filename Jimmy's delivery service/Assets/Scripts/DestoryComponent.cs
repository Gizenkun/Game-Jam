using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestoryComponent : MonoBehaviour
{
    private SpriteRenderer _sr;
    private float _startTime;
    private float _durationTime;
    private bool _start = false;

    private void Awake()
    {
        if(!TryGetComponent<SpriteRenderer>(out _sr))
        {
            _sr = GetComponentInChildren<SpriteRenderer>();
        }
    }

    private void Update()
    {
        if (!_start) return;
        Color tempColor = _sr.color;
        tempColor.a = 1 - (Time.time - _startTime) / _durationTime;
        _sr.color = tempColor;

        if(Time.time - _startTime >= _durationTime)
        {
            Destroy(this.gameObject);
        }
    }

    public void Destroy(float time)
    {
        _durationTime = time;
        _startTime = Time.time;
        _start = true;
    }
}
