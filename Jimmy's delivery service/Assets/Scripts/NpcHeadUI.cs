using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum NpcParcelState
{
    Leisure,
    Waiting,
    Timeout
}

public class NpcHeadUI : MonoBehaviour
{
    [SerializeField]
    private Text _nameText;
    [SerializeField]
    private Image _stateImage;

    [SerializeField]
    private List<Color> _stateColor = new List<Color>();

    private NpcParcelState _currentState;
    private Vector3 _wPos;

    public void Init(string npcName, Vector3 wPos)
    {
        _nameText.text = npcName;
        SetState(NpcParcelState.Leisure);
        _wPos = wPos;
    }

    private void Update()
    {
        this.transform.position = Camera.main.WorldToScreenPoint(_wPos);
    }

    public void SetState(NpcParcelState state)
    {
        _currentState = state;
        switch (state)
        {
            case NpcParcelState.Leisure:
                _stateImage.color = _stateColor[0];
                break;
            case NpcParcelState.Waiting:
                _stateImage.color = _stateColor[1];
                break;
            case NpcParcelState.Timeout:
                _stateImage.color = _stateColor[2];
                break;
            default:
                break;
        }
    }
}
