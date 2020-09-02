using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum NpcState
{
    Firm,//坚定的
    Shake,//动摇的
    Convinced,//被劝服的
    Angry,//生气的
    frightened//受惊的
}

public enum GengerType
{
    Male,
    Female
}

[Serializable]
public class NpcDialogue
{
    public NpcState key;
    public string dialogueContext;
}

[CreateAssetMenu(fileName = "NpcData", menuName = "Npc数据资源")]
public class NpcDataAsset : ScriptableObject
{
    //姓名
    public string npcName;
    //性别
    public GengerType gender;
    //形象资源
    public GameObject npcPrefab;
    //暴躁程度
    public int testiness;
    //活力
    public int vitality;
    //胆小程度
    public int courage;
    //npc对话数据
    public List<NpcDialogue> dialogueList;

    private void Awake()
    {
        if(dialogueList == null)
        {
            dialogueList = new List<NpcDialogue>();
            foreach (NpcState state in Enum.GetValues(typeof(NpcState)))
            {
                dialogueList.Add(new NpcDialogue() { key = state });
            }
        }
    }

    public string GetDialogue(NpcState state)
    {
        return dialogueList.Find(item => item.key == state)?.dialogueContext;
    }
}
