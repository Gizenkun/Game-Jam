using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
[CreateAssetMenu(fileName = "蜘蛛", menuName = "物品模板/蜘蛛")]
public class SpiderAsset : Item
{
    public ParcelBuffInfo buffInfo;
    public override void UseItem(PlayerController pc)
    {
        pc.ReceiveBuffToParcel(buffInfo);
    }
}