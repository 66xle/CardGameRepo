using SerializeReferenceEditor;
using System.Collections.Generic;
using UnityEngine;
using MyBox;

public enum ArmourSlot
{
    Head,
    Chest,
    Leg,
    Boot
}

public class ArmourData : GearData
{
    public ArmourSlot ArmourSlot;

    public override int Value => ArmourDefence;
    [ReadOnly] public int ArmourDefence;

    public override List<WeaponCardAnimationData> Cards => _cards;
    [SerializeReference][SR] public List<WeaponCardAnimationData> _cards;


    public ArmourData() { }

    public ArmourData(ArmourData data)
    {
        GearName = data.GearName;
        Description = data.Description;
        Rarity = data.Rarity;
        _cards = data._cards;
        Prefab = data.Prefab;
    }
}
