using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
[CreateAssetMenu(fileName = "相机", menuName = "物品模板/相机")]
public class CameraItemAsset : Item
{
    public override void UseItem(PlayerController pc)
    {
        pc.ReceiveItem(new PhotoAsset(), 1);
    }
}
