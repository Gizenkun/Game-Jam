using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class PlayerData
{
    public int money;
    public int satisfaction;
    public int performance;
}

public class ItemInfo
{
    public Item item;
    public int count;
}

public class PlayerBackpack
{
    private PlayerController _pc;
    private List<ItemInfo> _itemInfoList = new List<ItemInfo>();

    public List<ItemInfo> ItemInfoList
    {
        get
        {
            return _itemInfoList;
        }
    }

    public PlayerBackpack(PlayerController pc)
    {
        _pc = pc;
    }

    public void AddItem(Item item, int count)
    {
        for (int i = 0; i < _itemInfoList.Count; i++)
        {
            if(_itemInfoList[i].item.id == item.id)
            {
                _itemInfoList[i].count += count;
                return;
            }
        }
        _itemInfoList.Add(new ItemInfo() { item = item, count = count });
    }

    public void UseItem(int itemId)
    {
        bool remove = false;
        int index = -1;
        for (int i = 0; i < _itemInfoList.Count; i++)
        {
            if (_itemInfoList[i].item.id == itemId)
            {
                if(_itemInfoList[i].item.isConsumable)
                {
                    _itemInfoList[i].count--;
                    remove = _itemInfoList[i].count <= 0;
                    index = i;
                }
                _itemInfoList[i].item.UseItem(_pc);
                break;
            }
        }
        _itemInfoList.RemoveAt(index);
    }
}

public enum InteractionOption
{
    Suggest,
    Warn,
    UseItemForParcel,
    Nothing
}

[Serializable]
public class PlayerDialogInfo
{
    public string key;
    public List<string> contexts;
    public string GetRandomContext()
    {
        return contexts[UnityEngine.Random.Range(0, contexts.Count)];
    }
}

public class PlayerController : MonoBehaviour
{
    #region Component
    private Rigidbody2D _rigi;
    private Animator _animator;
    #endregion

    [SerializeField]
    private GameObject _model;
    [SerializeField]
    private GameObject _dialogPoint;

    #region Configuration
    [SerializeField]
    private float _speed = 5f;
    [SerializeField]
    private int _persuasion = 20;
    #endregion

    #region Input
    private float _horizontal;
    private float _vertical;
    #endregion

    public PlayerData _playerData;
    public PlayerBackpack _backpack;

    [SerializeField]
    private GameObject _parcelPrefab;
    private Dictionary<string, List<ParcelInfo>> _parcelDict = new Dictionary<string, List<ParcelInfo>>();

    public List<ParcelInfo> ParcelList
    {
        get
        {
            List<ParcelInfo> parcelList = new List<ParcelInfo>();
            foreach (var item in _parcelDict)
            {
                parcelList.AddRange(item.Value);
            }
            return parcelList;
        }
    }

    public ParcelInfo CurrentParcel
    {
        get
        {
            if(_parcelDict.ContainsKey(_currenParcelTargetName))
            {
                return _parcelDict[_currenParcelTargetName][0];
            }
            else
            {
                return null;
            }
        }
    }

    private string _currenParcelTargetName = string.Empty;

    [SerializeField]
    private List<PlayerDialogInfo> _dialogInfoList = new List<PlayerDialogInfo>();

    private bool _inInteractionState = false;


    public NpcController InteractionTarget
    {
        get;
        set;
    }

    [SerializeField]
    private GameObject _originPoint;

    public bool Lock
    {
        get;
        set;
    }

    void Awake()
    {
        _rigi = GetComponent<Rigidbody2D>();
        _animator = GetComponentInChildren<Animator>();

    }

    // Start is called before the first frame update
    void Start()
    {
        _playerData = new PlayerData()
        {
            money = 0,
            satisfaction = 50,
            performance = 100,
        };
        _backpack = new PlayerBackpack(this);
    }

    // Update is called once per frame
    void Update()
    {
        UIManager.Instance.GetUIPanel<StatePanel>().UpdateStateUI(_playerData, ParcelList.Count);
        foreach (var item in _parcelDict)
        {
            ParcelInfo timeoutParcel = null;
            foreach (var parcel in item.Value)
            {
                if(parcel.timeout)
                {
                    timeoutParcel = parcel;
                    continue;
                }
                parcel.UpdateState(Time.deltaTime);
            }
            if(timeoutParcel != null)
            {
                timeoutParcel.UpdateState(Time.deltaTime);
            }
        }

        if(Lock)
        {
            _horizontal = 0;
            _vertical = 0;
            return;
        }

        if (_inInteractionState)
        {
            _horizontal = 0;
            _vertical = 0;
            return;
        }
        _horizontal = Input.GetAxisRaw("Horizontal");
        _vertical = Input.GetAxisRaw("Vertical");

        if(Input.GetKeyDown(KeyCode.Z))
        {
            if(InteractionTarget != null)
            {
                if(!InteractionTarget.Leave)
                {
                    if (_parcelDict.ContainsKey(InteractionTarget.NpcName))
                    {
                        _currenParcelTargetName = InteractionTarget.NpcName;
                        _inInteractionState = true;
                        AudioManager.Instance.PlayAudio("Ringing", false, 1f);
                        UIManager.Instance.GetUIPanel<CommonPanel>().ShowOptionWindow(result =>
                        {
                            HandleOption(result);
                        });
                    }
                    else
                    {
                        UIManager.Instance.GetUIPanel<CommonPanel>().ShowCenterTip("此处没有包裹需要投递");
                    }
                }
                else
                {
                    if (_parcelDict.ContainsKey(InteractionTarget.NpcName))
                    {
                        _currenParcelTargetName = InteractionTarget.NpcName;
                        PutDownParcel();
                        UIManager.Instance.GetUIPanel<CommonPanel>().ShowCenterTip($"{InteractionTarget.NpcName}已经出门了");
                    }
                    else
                    {
                        UIManager.Instance.GetUIPanel<CommonPanel>().ShowCenterTip("此处没有包裹需要投递");
                    }
                }
            }
        }
    }

    private void UpdateParcelState()
    {

    }

    private void FixedUpdate()
    {
        Movement();
        UpdateAnimation();
    }

    private void Movement()
    {
        _rigi.velocity = new Vector2(_horizontal, _vertical).normalized * SquareToDiscMap(_horizontal, _vertical) * _speed;
        if(_rigi.velocity.x * _model.transform.localScale.x < 0)
        {
            _model.transform.localScale = new Vector3(-_model.transform.localScale.x, 1, 1);
        }
    }

    private void UpdateAnimation()
    {
        _animator.SetFloat("Speed", _rigi.velocity.magnitude);
    }

    private float SquareToDiscMap(float x, float y)
    {
        float u = x * Mathf.Sqrt(1 - Mathf.Pow(y, 2) / 2);
        float v = y * Mathf.Sqrt(1 - Mathf.Pow(x, 2) / 2);
        return Mathf.Sqrt(u * u + v * v);
    }

    public void ReceiveParcel(ParcelInfo parcel)
    {
        Debug.Log("GetParcel : " + parcel.targetNpcName);
        if(_parcelDict.ContainsKey(parcel.targetNpcName))
        {
            _parcelDict[parcel.targetNpcName].Add(parcel);
        }
        else
        {
            _parcelDict.Add(parcel.targetNpcName, new List<ParcelInfo>() { parcel });
        }
    }

    public void ReceiveItem(Item item, int count)
    {
        _backpack.AddItem(item, count);
    }

    public void ReceiveBuffToParcel(ParcelBuffInfo buffInfo)
    {
        CurrentParcel.buffInfo = buffInfo;
    }

    public void PersuasionUp(int upValue)
    {
        _persuasion += upValue;
    }

    public void HandleOption(InteractionOption option)
    {
        string gender = InteractionTarget.NpcGender == GengerType.Male ? "先生" : "女士";
        string context = string.Empty;
        switch (option)
        {
            case InteractionOption.Suggest:
                context = $"{InteractionTarget.NpcName}{gender},您的包裹,{GetDialog("Suggest")}";
                UIManager.Instance.GetUIPanel<CommonPanel>().ShowDialogWindow(context, context.Length / 10f, () =>
                {
                    InteractionTarget.Knock(KnockEventType.SoftTalk, _persuasion, () =>
                    {
                        PutDownParcel();
                        _inInteractionState = false;
                    });
                }, _dialogPoint.transform.position);
                break;
            case InteractionOption.Warn:
                context = $"{InteractionTarget.NpcName}{gender},您的包裹,{GetDialog("Warn")}";
                UIManager.Instance.GetUIPanel<CommonPanel>().ShowDialogWindow(context, context.Length / 10f, () =>
                {
                    InteractionTarget.Knock(KnockEventType.StrongTalk, _persuasion, () =>
                    {
                        PutDownParcel();
                        _inInteractionState = false;
                    });
                }, _dialogPoint.transform.position);
                break;
            case InteractionOption.UseItemForParcel:
                UIManager.Instance.GetUIPanel<BackpackPanel>().Show(_backpack.ItemInfoList.FindAll(item => item.item.canAddToParcel), (id) =>
                {
                    Debug.Log(id);
                    _backpack.UseItem(id);
                    PutDownParcel();
                    _inInteractionState = false;
                }, () =>
                {
                    UIManager.Instance.GetUIPanel<CommonPanel>().ShowOptionWindow(result =>
                    {
                        HandleOption(result);
                    });
                });
                break;
            case InteractionOption.Nothing:
                PutDownParcel();
                _inInteractionState = false;
                break;
            default:
                break;
        }
    }

    private string GetDialog(string key)
    {
        foreach (var item in _dialogInfoList)
        {
            if (item.key == key)
                return item.GetRandomContext();
        }
        return string.Empty;
    }

    public void PutDownParcel()
    {
        ParcelObj obj = Instantiate(_parcelPrefab, transform.position, Quaternion.identity).GetComponent<ParcelObj>();
        obj.BuffInfo = CurrentParcel.buffInfo;
        //List<ParcelInfo> removeList = new List<ParcelInfo>();
        //for (int i = 0; i < _parcelList.Count; i++)
        //{
        //    if(_parcelList[i].targetNpcName == CurrentParcel.targetNpcName)
        //    {
        //        removeList.Add(_parcelList[i]);
        //    }
        //}

        //foreach (var item in removeList)
        //{
        //    _parcelList.Remove(item);
        //}
        GameManager.Instance.DeliverParcel(_parcelDict[CurrentParcel.targetNpcName].Count);
        _parcelDict.Remove(CurrentParcel.targetNpcName);
    }

    public void Reset()
    {
        transform.position = _originPoint.transform.position;
        _parcelDict.Clear();
        _currenParcelTargetName = string.Empty;
        _inInteractionState = false;
        InteractionTarget = null;
    }
}
