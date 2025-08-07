using UnityEngine;
using MyBox;
using SerializeReferenceEditor;
using System.Collections.Generic;

public class GearData : ScriptableObject
{
    [ReadOnly] public string Guid;

    public string GearName;
    public string Description;
    public virtual int Value { get; }
    public GameObject Prefab;
    public Rarity Rarity;

    public virtual List<WeaponCardData> Cards { get; }
}
