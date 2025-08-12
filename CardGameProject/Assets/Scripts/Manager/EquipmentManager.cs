using MyBox;
using System.Collections;
using System.Collections.Generic;
using Unity.Android.Gradle.Manifest;
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


    private void Awake()
    {
        LoadWeapons();
    }

    public void SaveWeapons()
    {
        GameManager.Instance.MainHand = MainHand;
        GameManager.Instance.OffHand = OffHand;
        GameManager.Instance.EquippedWeapons = _equippedWeapons;

        GameManager.Instance.IsWeaponsSaved = true;
    }

    public void LoadWeapons()
    {
        _equippedWeapons = GameManager.Instance.IsWeaponsSaved ? GameManager.Instance.EquippedWeapons : EquippedWeapons;
    }

    public void AddWeapon(WeaponData weapon)
    {
        _equippedWeapons.Add(weapon);
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
