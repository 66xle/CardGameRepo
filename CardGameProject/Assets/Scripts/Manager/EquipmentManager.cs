using MyBox;
using System.Collections;
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
}
