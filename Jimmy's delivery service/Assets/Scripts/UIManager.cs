using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [SerializeField]
    private EventSystem _eventSystem;

    private List<UIPanel> _uipanelList = new List<UIPanel>();

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
            Destroy(_eventSystem.gameObject);
        }
        DontDestroyOnLoad(this.gameObject);
        DontDestroyOnLoad(_eventSystem.gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void WorldToUI(Vector3 wpos)
    {
        this.transform.position = Camera.main.WorldToScreenPoint(wpos);
    }

    public void AddUIPanel(UIPanel uipanel)
    {
        _uipanelList.Add(uipanel);
    }

    public T GetUIPanel<T>() where T : class
    {
        foreach (var uipanel in _uipanelList)
        {
            if(uipanel is T)
            {
                return uipanel as T;
            }
        }
        return null;
    }

    public void Reset()
    {
        foreach (var uipanel in _uipanelList)
        {
            uipanel.Reset();
        }
    }

    public void Reload()
    {
        Reset();
    }
}

public abstract class UIPanel : MonoBehaviour
{
    public virtual void Start()
    {
        UIManager.Instance.AddUIPanel(this);
    }

    public virtual void Show()
    {

    }

    public virtual void Hide()
    {

    }

    public virtual void Reset()
    {

    }
}