using MyBox;
using System.Collections.Generic;
using UnityEngine;

public class GearData : ScriptableObject
{
    [ReadOnly] public string Guid;

    public string GearName;
    public string Description;
    public virtual int Value { get; }
    public GameObject Prefab;
    public Texture IconTexture; // Reward Manager
    public Rarity Rarity;
    public bool Passive = false;
    
    public virtual List<CardAnimationData> Cards { get; }
}
