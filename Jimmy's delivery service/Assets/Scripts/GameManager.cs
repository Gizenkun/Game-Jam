using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Data;
using UnityEngine.SceneManagement;

public class TimeData
{
    public int hours;
    public int minutes;
    public int seconds;
}

public enum LuckEventType
{
    RandomReceiveItem,
    ReceiveMoney,
    PersuasionUp,
    Nothing
}

[Serializable]
public class LuckEventInfo
{
    public LuckEventType type;
    public int weight;
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField]
    private List<Item> _itemAssetList = new List<Item>();
    [SerializeField]
    private List<NpcDataAsset> _npcDataAsset = new List<NpcDataAsset>();
    [SerializeField]
    private List<string> _tipsList = new List<string>();

    [SerializeField]
    private float _lapsesSpeed = 500f;
    [SerializeField]
    private float _originDistributeInterval = 3;//单位hours
    [SerializeField]
    private float _intervalDecreaseFactor = 0.9f;
    [SerializeField]
    private int _originDistributeCount = 3;
    [SerializeField]
    private float _countIncreaseFactor = 1.2f;

    private int DistributeInterval
    {
        get
        {
            return Mathf.Max(1, (int)(_originDistributeInterval * Mathf.Pow(_intervalDecreaseFactor, _currentDay - 1)));
        }
    }

    private int DistributeCount
    {
        get
        {
            return Mathf.Max(1, (int)(_originDistributeCount * Mathf.Pow(_countIncreaseFactor, _currentDay - 1)));
        }
    }

    [SerializeField]
    private List<LuckEventInfo> _luckEventList = new List<LuckEventInfo>();

    private int _currentSeconds;
    public TimeData CurrentTime
    {
        get
        {
            TimeData timeData = GetTime();
            timeData.hours += 6;
            return timeData;
        }
    }

    private int _currentDay = 0;
    public int CurrentDay
    {
        get
        {
            return _currentDay;
        }
    }

    [SerializeField]
    private PlayerController _pc;
    [SerializeField]
    private NpcManager _npcManager;

    private bool _stopTime = false;
    private int _lastHours = 0;

    private bool _inited = false;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Instance.Reload();
            Destroy(this.gameObject);
        }
        DontDestroyOnLoad(this);
    }

    private void Start()
    {
        if(_pc == null)
        {
            _pc = GameObject.FindObjectOfType<PlayerController>();
        }
        if(_npcManager == null)
        {
            _npcManager = GameObject.FindObjectOfType<NpcManager>();
        }
        _lastHours = - (DistributeInterval + 1);
        DayEnd();
        AudioManager.Instance.PlayAudio("Bgm03", true, 0f);
        _inited = true;
    }

    private void Reset()
    {
        _currentDay = 0;
        _currentSeconds = 0;
        _stopTime = false;
        _lastHours = -(DistributeInterval + 1);
        if (_pc == null)
        {
            _pc = GameObject.FindObjectOfType<PlayerController>();
        }
        if (_npcManager == null)
        {
            _npcManager = GameObject.FindObjectOfType<NpcManager>();
        }
        DayEnd();
    }

    public void Update()
    {
        if(!_inited)
        {
            Reset();
            _inited = true;
        }
        if(!_stopTime)
        {
            TimeLapses();
            if (GetTime().hours >= 12)
            {
                DayEnd();
            }
        }

        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    private void TimeLapses()
    {
        _currentSeconds += (int)(Time.deltaTime * _lapsesSpeed);
        int currentHours = GetTime().hours;
        if (currentHours - _lastHours >= DistributeInterval)
        {
            DistributeParcel();
            _lastHours = currentHours;
        }
    }

    private TimeData GetTime()
    {
        int seconds = _currentSeconds % 60;
        int minutes = _currentSeconds / 60;
        int hours = minutes / 60;
        minutes = minutes % 60;
        return new TimeData() { hours = hours, minutes = minutes, seconds = seconds };
    }

    private void DayStart()
    {
        _lastHours = -(DistributeInterval + 1);
        //UIManager.Instance.Reset();
        //_pc.Reset();
        //_npcManager.Reset();
        _stopTime = false;
        _pc.Lock = false;
    }

    private void DayEnd()
    {
        Debug.Log(DistributeInterval + "  :  " + DistributeCount);
        UIManager.Instance.Reset();
        _pc.Reset();
        _pc.Lock = true;
        _npcManager.Reset();
        _currentDay++;
        _currentSeconds = 0;
        _stopTime = true;
        ChangeSatisfactionValue(-5);
        Cutscene.Instance.PlayCutscene(() =>
        {
            Time.timeScale = 0;
            LuckEvent(() =>
            {
                Time.timeScale = 1;
                DayStart();
            });
        });
    }

    private void DistributeParcel()
    {
        if(_pc != null)
        {
            for (int i = 0; i < DistributeCount; i++)
            {
                _pc.ReceiveParcel(new ParcelInfo() { targetNpcName = _npcDataAsset[UnityEngine.Random.Range(0, _npcDataAsset.Count)].npcName, timelimit = UnityEngine.Random.Range(10f, 20f) });
            }
        }
    }

    public void DeliverParcel(int deliverCount)
    {
        ChangeMoneyValue(deliverCount);
    }

    public void NegativeFeedback(int count)
    {
        ChangePerformanceValue(-count);
    }

    public void NpcGoAwayFromHome(string npcName)
    {
        ChangeSatisfactionValue(10);
    }

    private void ChangeMoneyValue(int changeValue)
    {
        _pc._playerData.money += changeValue;
        _pc._playerData.money = Mathf.Max(0, _pc._playerData.money);
    }

    private void ChangePerformanceValue(int changeValue)
    {
        _pc._playerData.performance += changeValue;
        _pc._playerData.performance = Mathf.Clamp(_pc._playerData.performance, 0, 100);

        if (_pc._playerData.performance <= 0)
        {
            //TODO : 游戏结束 被辞退
            HandleFail(0);
        }
    }

    private void ChangeSatisfactionValue(int changeValue)
    {
        _pc._playerData.satisfaction += changeValue;
        _pc._playerData.satisfaction = Mathf.Clamp(_pc._playerData.satisfaction, 0, 100);

        if(_pc._playerData.satisfaction <= 0)
        {
            //TODO : 游戏结束  辞职
            HandleFail(1);
        }
    }

    private void HandleFail(int type)
    {
        Time.timeScale = 0;
        if(type == 0)
        {
            UIManager.Instance.GetUIPanel<CommonPanel>().ShowCenterTip("太遗憾了，Jimmy遭受了太多差评，被辞退了", "确定", () =>
            {
                Time.timeScale = 1;
                SceneManager.LoadScene(0);
            });
        }
        else
        {
            UIManager.Instance.GetUIPanel<CommonPanel>().ShowCenterTip("Jimmy心情低落到了极低，他认为他或许没有能力改变这个现状，他决定离开这份工作", "确定", () =>
            {
                Time.timeScale = 1;
                SceneManager.LoadScene(0);
            });
        }
    }

    private void LuckEvent(Action callback)
    {
        HandleLuckEvent(GetLuckEvent(), callback);
    }

    private LuckEventType GetLuckEvent()
    {
        float dice = (float)new System.Random().NextDouble();
        Debug.Log(dice);

        int sumWeight = 0;
        foreach (var item in _luckEventList)
        {
            sumWeight += item.weight;
        }

        Debug.Log("sumWeight  " + sumWeight);

        foreach (var item in _luckEventList)
        {
            dice -= (float)item.weight / sumWeight;
            Debug.Log(dice);
            if (dice <= 0)
            {
                return item.type;
            }
        }
        return LuckEventType.Nothing;
    }

    private void HandleLuckEvent(LuckEventType eventType, Action callback)
    {
        switch (eventType)
        {
            case LuckEventType.RandomReceiveItem:
                Item item = _itemAssetList[UnityEngine.Random.Range(0, _itemAssetList.Count)];
                _pc.ReceiveItem(item, 1);
                UIManager.Instance.GetUIPanel<CommonPanel>().ShowCenterTip($"幸运的捡到了一个{item.name}，好好使用吧!", "确认", callback);
                break;
            case LuckEventType.ReceiveMoney:
                int count = UnityEngine.Random.Range(2, 10);
                ChangeMoneyValue(count);
                UIManager.Instance.GetUIPanel<CommonPanel>().ShowCenterTip($"幸运的捡到了{count}个硬币，好好使用吧!", "确认", callback);
                break;
            case LuckEventType.PersuasionUp:
                _pc.PersuasionUp(UnityEngine.Random.Range(2, 10));
                UIManager.Instance.GetUIPanel<CommonPanel>().ShowCenterTip($"看到了一本好书，人格魅力与口才得到了提升，相信现在能更容易的说服别人了吧!", "确认", callback);
                break;
            case LuckEventType.Nothing:
                UIManager.Instance.GetUIPanel<CommonPanel>().ShowCenterTip($"什么都没有发生...{ _tipsList[UnityEngine.Random.Range(0, _tipsList.Count)]}", "确认", callback);
                break;
            default:
                break;
        }
    }
    public void Reload()
    {
        _inited = false;
    }
}
