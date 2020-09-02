using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class DialogWindow : MonoBehaviour
{
    [SerializeField]
    private Text _text;

    private bool _follow = false;
    private GameObject _followTarget;
    private Vector3 _offset;
    private Vector3 _targetPos;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void ShowDialogWindow(string context, float time, Action callback, Vector3 wPos, bool follow = false, GameObject followTarget = null, Vector3 offset = default(Vector3))
    {
        AudioManager.Instance.PlayAudio("OnePoint04", false, 1f);
        gameObject.SetActive(true);
        _text.text = context;
        _follow = follow;
        if(follow)
        {
            _followTarget = followTarget;
            _offset = offset;
        }
        else
        {
            _targetPos = wPos;
        }
        StartCoroutine(Tick(time, callback));
    }

    // Update is called once per frame
    void Update()
    {
        if(_follow)
        {
            _targetPos = _followTarget.transform.position + _offset;
        }
        this.transform.position = Camera.main.WorldToScreenPoint(_targetPos);
    }

    private IEnumerator Tick(float time, Action callback)
    {
        yield return new WaitForSeconds(time);
        gameObject.SetActive(false);
        callback?.Invoke();
    }
}
