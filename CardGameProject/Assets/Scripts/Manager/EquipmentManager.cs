using MyBox;
using System.Collections.Generic;
using UnityEngine;

public enum ArmourType
{
    None,
    Light,
    Medium,
    Heavy
}

public enum DamageType
{
    Slash,
    Pierce,
    Blunt
}


public class EquipmentManager : MonoBehaviour
{
    [SerializeField] ArmourData Head;
    [SerializeField] ArmourData Shoulders;
    [SerializeField] ArmourData Arms;
    [SerializeField] ArmourData Chest;
    [SerializeField] ArmourData Legs;
    [SerializeField] ArmourData Boots;

    [Separator]

    [MustBeAssigned] public WeaponData MainHand;
    public WeaponData OffHand;

    [Separator]

    [SerializeField] List<WeaponData> EquippedWeapons;
    private List<WeaponData> _equippedWeapons;

    public WeaponData FixedWeapon1stBattle;
    public WeaponData FixedWeapon2ndBattle;

    private void Awake()
    {
        SceneInitialize.Instance.Subscribe(LoadGear);
    }


    public void SaveGear()
    {
        GameManager.Instance.MainHand = MainHand;
        GameManager.Instance.OffHand = OffHand;
        GameManager.Instance.EquippedWeapons = _equippedWeapons;
        GameManager.Instance.EquippedArmour = new() { Head, Shoulders, Arms, Chest, Legs, Boots };

        GameManager.Instance.IsEquipmentSaved = true;
    }

    public void LoadGear()
    {
        _equippedWeapons = GameManager.Instance.IsEquipmentSaved ? GameManager.Instance.EquippedWeapons : EquippedWeapons;

        Head        = GameManager.Instance.IsEquipmentSaved ? GameManager.Instance.EquippedArmour[0] : Head;
        Shoulders   = GameManager.Instance.IsEquipmentSaved ? GameManager.Instance.EquippedArmour[1] : Shoulders;
        Arms        = GameManager.Instance.IsEquipmentSaved ? GameManager.Instance.EquippedArmour[2] : Arms;
        Chest       = GameManager.Instance.IsEquipmentSaved ? GameManager.Instance.EquippedArmour[3] : Chest;
        Legs        = GameManager.Instance.IsEquipmentSaved ? GameManager.Instance.EquippedArmour[4] : Legs;
        Boots       = GameManager.Instance.IsEquipmentSaved ? GameManager.Instance.EquippedArmour[5] : Boots;
    }

    public void AddGear(GearData gear)
    {
        if (gear is WeaponData)
        {
            _equippedWeapons.Add(gear as WeaponData);
        }
        else if (gear is ArmourData)
        {
            ArmourData armourData = gear as ArmourData;

            if (armourData.ArmourSlot == ArmourSlot.Head) Head = armourData;
            else if (armourData.ArmourSlot == ArmourSlot.Shoulders) Head = armourData;
            else if (armourData.ArmourSlot == ArmourSlot.Arms) Arms = armourData;
            else if (armourData.ArmourSlot == ArmourSlot.Chest) Chest = armourData;
            else if (armourData.ArmourSlot == ArmourSlot.Legs) Legs = armourData;
            else if (armourData.ArmourSlot == ArmourSlot.Boots) Boots = armourData;
        }
    }

    public List<WeaponData> GetEquippedWeapons()
    {
        return _equippedWeapons;
    }

    public List<ArmourData> GetEquippedArmours()
    {
        List<ArmourData> armourDatas = new() { Head, Shoulders, Arms, Chest, Legs, Boots };

        List<ArmourData> equipped = new();

        foreach (ArmourData armourData in armourDatas)
        {
            if (armourData == null) continue;

            equipped.Add(armourData);
        }

        return equipped;
    }

    public float GetArmoursDefence()
    {
        float defence = 0;

        List<ArmourData> armourDatas = new() { Head, Shoulders, Arms, Chest, Legs, Boots };

        foreach (ArmourData data in armourDatas)
        {
            if (data == null) continue;

            defence += data.ArmourDefence;
        }

        return defence;
    }
}
