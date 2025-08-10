using SerializeReferenceEditor;
using System.Collections.Generic;
using UnityEngine;
using MyBox;
using System.Linq;
using UnityEditor;

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

    public override List<CardAnimationData> Cards => _cards;
    [SerializeReference][SR] public List<CardAnimationData> _cards;


    public ArmourData() { }

    public ArmourData(ArmourData data)
    {
        GearName = data.GearName;
        Description = data.Description;
        Rarity = data.Rarity;
        _cards = data._cards;
        Prefab = data.Prefab;
    }

#if UNITY_EDITOR
    public void OnValidate()
    {
        foreach (CardAnimationData data in _cards)
        {
            if (data == null) continue;

            data.UpdateAnimationList();
        }
    }
#endif
}
