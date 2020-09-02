using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class NpcData
{
    //提升对象愤怒值
    public int angry;
    //提升对象活力值
    public int vitality;
    //惊吓值
    public int scare;
}

public enum KnockEventType
{
    NoneTalk,
    SoftTalk,
    StrongTalk
}

public class NpcController : MonoBehaviour
{
    [SerializeField]
    private NpcDataAsset _npcDataAsset;
    [SerializeField]
    private Trigger _interactionTrigger;
    [SerializeField]
    private GameObject _smoke;
    [SerializeField]
    private SpriteRenderer _doorSr;
    [SerializeField]
    private Sprite _closeSprite;
    [SerializeField]
    private Sprite _openSprite;
    [SerializeField]
    private GameObject _npcPoint;

    [SerializeField]
    private NpcData _npcData;

    private int _knockCount = 0;

    private bool _leave;

    public bool Leave
    {
        get
        {
            return _leave;
        }
    }

    public string NpcName
    {
        get
        {
            return _npcDataAsset.npcName;
        }
    }

    public GengerType NpcGender
    {
        get
        {
            return _npcDataAsset.gender;
        }
    }

    private void Start()
    {
        _npcData = new NpcData();
        _interactionTrigger.Active = true;
        _interactionTrigger.TriggerEnterAction = (other) =>
        {
            UIManager.Instance.GetUIPanel<CommonPanel>().ShowBottomTip("点击Z进行交互");
            Debug.Log(other.gameObject);
            if(other.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                PlayerController pc = other.GetComponent<PlayerController>();
                pc.InteractionTarget = this;
            }
            if(other.gameObject.layer == LayerMask.NameToLayer("Parcel"))
            {
                ParcelObj obj = other.GetComponent<ParcelObj>();
                PickUpParcel(obj);
            }
        };
        _interactionTrigger.TriggerExitAction = (other) =>
        {
            UIManager.Instance.GetUIPanel<CommonPanel>().HideBottomTip();
            if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                PlayerController pc = other.GetComponent<PlayerController>();
                if(pc.InteractionTarget == this)
                {
                    pc.InteractionTarget = null;
                }
            }
        };
        Init();
    }

    public void Init()
    {
        _doorSr.sprite = _closeSprite;
        _smoke.SetActive(UnityEngine.Random.Range(0, 2) == 1);
        _knockCount = 0;
        _leave = false;
    }

    public void Knock(KnockEventType eventType, int persuasion, Action callback)
    {
        if (_leave) return;
        int vitalityAdvance;
        int angryAdvance;
        switch (eventType)
        {
            case KnockEventType.NoneTalk:
                break;
            case KnockEventType.SoftTalk:
                vitalityAdvance = (int)(persuasion * (_npcDataAsset.vitality / 100.0f) * (1 - 0.2f * _knockCount));
                angryAdvance = (int )((10f * _knockCount) * (_npcDataAsset.testiness / 100.0f));
                _npcData.vitality += vitalityAdvance;
                _npcData.angry += angryAdvance;
                break;
            case KnockEventType.StrongTalk:
                vitalityAdvance = (int)(persuasion * (_npcDataAsset.vitality / 100.0f) * 1.2f);
                angryAdvance = (int)((20 + (10f * _knockCount)) * (_npcDataAsset.testiness / 100.0f));
                _npcData.vitality += vitalityAdvance ;
                _npcData.angry += angryAdvance;
                break;
            default:
                break;
        }
        _knockCount++;
        HandleReaction(eventType, callback);
    }

    private void PickUpParcel(ParcelObj parcelObj)
    {
        if (_leave)
        {
            parcelObj.PickUp();
            HeadUIManager.Instance.GetNpcHeadUI(NpcName).SetState(NpcParcelState.Leisure);
            return;
        }
        int vitalityAdvance;
        int angryAdvance;
        int scareAdvace;
        vitalityAdvance = (int)(parcelObj.BuffInfo.vitality * (_npcDataAsset.vitality / 100.0f));
        angryAdvance = (int)(parcelObj.BuffInfo.angry * (_npcDataAsset.testiness / 100.0f));
        scareAdvace = (int)(parcelObj.BuffInfo.scare * (_npcDataAsset.courage / 100.0f));
        _npcData.vitality += vitalityAdvance;
        _npcData.angry += angryAdvance;
        _npcData.scare += scareAdvace;
        parcelObj.PickUp();
        HandleReaction(KnockEventType.NoneTalk, null);
        HeadUIManager.Instance.GetNpcHeadUI(NpcName).SetState(NpcParcelState.Leisure);
    }

    private void HandleReaction(KnockEventType eventType, Action callback)
    {
        if(_npcData.scare >= 100)
        {
            OpenDoor();
            string context = _npcDataAsset.GetDialogue(NpcState.frightened);
            UIManager.Instance.GetUIPanel<CommonPanel>().ShowDialogWindow(context, context.Length / 10f, () =>
            {
                callback?.Invoke();
            }, transform.position);
            return;
        }

        if(_npcData.vitality >= 100)
        {
            OpenDoor();
            string context = _npcDataAsset.GetDialogue(NpcState.Convinced);
            UIManager.Instance.GetUIPanel<CommonPanel>().ShowDialogWindow(context, context.Length / 10f, () =>
            {
                callback?.Invoke();
            }, transform.position);
            return;
        }


        if(_npcData.angry >= 100)
        {
            string context = _npcDataAsset.GetDialogue(NpcState.Angry);
            UIManager.Instance.GetUIPanel<CommonPanel>().ShowDialogWindow(context, context.Length / 10f, () =>
            {
                callback?.Invoke();
            }, transform.position);
            _npcData.angry -= 50;
            GameManager.Instance.NegativeFeedback(5);
            return;
        }

        if(eventType == KnockEventType.SoftTalk || eventType == KnockEventType.StrongTalk)
        {
            if(_npcData.vitality < 50)
            {
                string context = _npcDataAsset.GetDialogue(NpcState.Firm);
                UIManager.Instance.GetUIPanel<CommonPanel>().ShowDialogWindow(context, context.Length / 10f, () =>
                {
                    callback?.Invoke();
                }, transform.position);
            }

            else
            {
                string context = _npcDataAsset.GetDialogue(NpcState.Shake);
                UIManager.Instance.GetUIPanel<CommonPanel>().ShowDialogWindow(context, context.Length / 10f, () =>
                {
                    callback?.Invoke();
                }, transform.position);
            }
            return;
        }
    }

    private void OpenDoor()
    {
        _npcData.vitality = 0;
        _npcData.angry = 0;
        _npcData.scare = 0;
        AudioManager.Instance.PlayAudio("OpenDoor", false, 1f);
        _doorSr.sprite = _openSprite;
        GameObject npcObj = Instantiate(_npcDataAsset.npcPrefab, _npcPoint.transform);
        npcObj.GetComponent<DestoryComponent>().Destroy(5.0f);
        _leave = true;
        GameManager.Instance.NpcGoAwayFromHome(NpcName);
    }

    //public void 
}
