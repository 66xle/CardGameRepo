using SerializeReferenceEditor;
using System.Collections.Generic;
using UnityEngine;
using MyBox;

public class ArmourData : GearData
{
    public override int Value => ArmourDefence;
    [ReadOnly] public int ArmourDefence;

    public override List<WeaponCardData> Cards => _cards;
    [SerializeReference][SR] public List<WeaponCardData> _cards;


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
