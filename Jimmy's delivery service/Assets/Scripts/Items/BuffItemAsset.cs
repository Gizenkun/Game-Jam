using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
[CreateAssetMenu(fileName = "附加物品", menuName = "物品模板/附加物品")]
public class BuffItemAsset : Item
{
    public ParcelBuffInfo buffInfo;
    public override void UseItem(PlayerController pc)
    {
        pc.ReceiveBuffToParcel(buffInfo);
    }
}