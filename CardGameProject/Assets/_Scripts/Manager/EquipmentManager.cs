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
    private List<GearRuntime> _equippedWeapons;

    public WeaponData FixedWeapon1stBattle;
    public WeaponData FixedWeapon2ndBattle;

    [Separator]

    [MustBeAssigned] public StatsManager StatsManager;

    private void Awake()
    {
        SceneInitialize.Instance.Subscribe(LoadGear);
    }


    public void SaveGear()
    {
        Debug.Log("Save Gear");

        GameManager.Instance.MainHand = MainHand == null ? null : new GearRuntime(MainHand, StatsManager);
        GameManager.Instance.OffHand = OffHand == null ? null : new GearRuntime(OffHand, StatsManager);
        GameManager.Instance.EquippedWeapons = _equippedWeapons;
        GameManager.Instance.EquippedArmour = new() { new GearRuntime(Head, StatsManager),
                                                      new GearRuntime(Shoulders, StatsManager),
                                                      new GearRuntime(Arms, StatsManager),
                                                      new GearRuntime(Chest, StatsManager),
                                                      new GearRuntime(Legs, StatsManager),
                                                      new GearRuntime(Boots, StatsManager) };

        GameManager.Instance.IsEquipmentSaved = true;
    }

    public void LoadGear()
    {
        Debug.Log("Load Gear");

        if (GameManager.Instance.IsEquipmentSaved)
            _equippedWeapons = GameManager.Instance.EquippedWeapons;
        else
        {
            // Convert EquippedWeapons (List<WeaponData>) to List<GearRuntime>
            _equippedWeapons = new List<GearRuntime>();
            if (EquippedWeapons != null)
            {
                foreach (var weapon in EquippedWeapons)
                {
                    if (weapon == null) continue;

                    _equippedWeapons.Add(new GearRuntime(weapon, StatsManager));
                }
            }
        }

        Head = GameManager.Instance.IsEquipmentSaved ? GameManager.Instance.EquippedArmour[0].GearData as ArmourData : Head;
        Shoulders = GameManager.Instance.IsEquipmentSaved ? GameManager.Instance.EquippedArmour[1].GearData as ArmourData : Shoulders;
        Arms = GameManager.Instance.IsEquipmentSaved ? GameManager.Instance.EquippedArmour[2].GearData as ArmourData : Arms;
        Chest = GameManager.Instance.IsEquipmentSaved ? GameManager.Instance.EquippedArmour[3].GearData as ArmourData : Chest;
        Legs = GameManager.Instance.IsEquipmentSaved ? GameManager.Instance.EquippedArmour[4].GearData as ArmourData : Legs;
        Boots = GameManager.Instance.IsEquipmentSaved ? GameManager.Instance.EquippedArmour[5].GearData as ArmourData : Boots;
    }

    public void AddGear(GearRuntime gearRuntime)
    {
        if (gearRuntime.GearData is WeaponData)
        {
            _equippedWeapons.Add(gearRuntime);
        }
        else if (gearRuntime.GearData is ArmourData)
        {
            ArmourData armourData = gearRuntime.GearData as ArmourData;
            if (armourData.ArmourSlot == ArmourSlot.Head) Head = armourData;
            else if (armourData.ArmourSlot == ArmourSlot.Shoulders) Head = armourData;
            else if (armourData.ArmourSlot == ArmourSlot.Arms) Arms = armourData;
            else if (armourData.ArmourSlot == ArmourSlot.Chest) Chest = armourData;
            else if (armourData.ArmourSlot == ArmourSlot.Legs) Legs = armourData;
            else if (armourData.ArmourSlot == ArmourSlot.Boots) Boots = armourData;
        }
    }

    public List<GearRuntime> GetEquippedWeapons()
    {
        return _equippedWeapons;
    }

    public List<GearRuntime> GetEquippedArmours()
    {
        List<GearRuntime> armourGearRuntime = new() { new GearRuntime(Head, StatsManager),
                                                      new GearRuntime(Shoulders, StatsManager),
                                                      new GearRuntime(Arms, StatsManager),
                                                      new GearRuntime(Chest, StatsManager),
                                                      new GearRuntime(Legs, StatsManager),
                                                      new GearRuntime(Boots, StatsManager) };

        List<GearRuntime> equipped = new();
        foreach (GearRuntime armourData in armourGearRuntime)
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
