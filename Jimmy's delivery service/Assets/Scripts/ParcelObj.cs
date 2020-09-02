using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class ParcelBuffInfo
{
    //提升对象愤怒值
    public int angry;
    //提升对象活力值
    public int vitality;
    //惊吓值
    public int scare;

    public void Copy(ParcelBuffInfo parcelBuffInfo)
    {
        angry = parcelBuffInfo.angry;
        vitality = parcelBuffInfo.vitality;
        scare = parcelBuffInfo.scare;
    }
}

public class ParcelInfo
{
    public string targetNpcName;
    public float timelimit;
    public ParcelBuffInfo buffInfo = new ParcelBuffInfo();

    private float _useTime;
    private float _feedbackTime;
    public bool timeout = false;

    public void UpdateState(float deltaTime)
    {
        _useTime += deltaTime;
        NpcHeadUI headUi = HeadUIManager.Instance.GetNpcHeadUI(targetNpcName);
        if (_useTime <= timelimit)
        {
            headUi.SetState(NpcParcelState.Waiting);
        }
        else
        {
            //Debug.Log(targetNpcName);
            timeout = true;
            headUi.SetState(NpcParcelState.Timeout);
            _feedbackTime += deltaTime;
            //Debug.Log(_feedbackTime);
            //超时一秒给一个差评
            if (_feedbackTime > 1.0f)
            {
                GameManager.Instance.NegativeFeedback(1);
                _feedbackTime = 0;
            }
        }
    }
}

public class ParcelObj : MonoBehaviour
{
    [SerializeField]
    private float _releaseTime = 0.5f;

    private Collider2D _collider;

    private ParcelBuffInfo _buffInfo;
    public ParcelBuffInfo BuffInfo
    {
        get;
        set;
    }

    private void Awake()
    {
        _collider = GetComponent<Collider2D>();
    }

    public void PickUp()
    {
        _collider.enabled = false;
        GetComponent<DestoryComponent>().Destroy(_releaseTime);
    }
}