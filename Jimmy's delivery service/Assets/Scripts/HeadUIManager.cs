using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadUIManager : MonoBehaviour
{
    public static HeadUIManager Instance;

    private Dictionary<string, NpcHeadUI> _npcHeadUIDict = new Dictionary<string, NpcHeadUI>();
    [SerializeField]
    private GameObject _npcHeadUIPrefab;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
    }

    public void CreateNpcHeadUI(List<NpcController> npcList)
    {
        _npcHeadUIDict.Clear();
        foreach (var npc in npcList)
        {
            NpcHeadUI headUI = Instantiate(_npcHeadUIPrefab, this.transform).GetComponent<NpcHeadUI>();
            headUI.Init(npc.NpcName, npc.transform.position + Vector3.up * 4);
            _npcHeadUIDict.Add(npc.NpcName, headUI);
        }
    }

    public NpcHeadUI GetNpcHeadUI(string npcName)
    {
        return _npcHeadUIDict[npcName];
    }
}
