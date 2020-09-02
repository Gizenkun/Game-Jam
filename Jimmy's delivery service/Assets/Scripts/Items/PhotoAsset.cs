using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
[CreateAssetMenu(fileName = "相片", menuName = "物品模板/相片")]
public class PhotoAsset : Item
{
    public ParcelBuffInfo buffInfo;
    public override void UseItem(PlayerController pc)
    {
        pc.ReceiveBuffToParcel(buffInfo);
    }
}