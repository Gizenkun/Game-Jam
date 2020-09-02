using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class NpcManager : MonoBehaviour
{
    private List<NpcController> _npcControllerList = new List<NpcController>();

    public void Start()
    {
        _npcControllerList = transform.GetComponentsInChildren<NpcController>().ToList();
        Debug.Log(_npcControllerList.Count);
        HeadUIManager.Instance.CreateNpcHeadUI(_npcControllerList);
    }

    public void Reset()
    {
        foreach (var npc in _npcControllerList)
        {
            npc.Init();
            HeadUIManager.Instance.GetNpcHeadUI(npc.NpcName).SetState(NpcParcelState.Leisure);
        }
    }
}
