using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public abstract class Item : ScriptableObject
{
    public int id;
    public int price;
    public bool isConsumable;
    public bool canAddToParcel;
    public string description;
    public abstract void UseItem(PlayerController pc);
}