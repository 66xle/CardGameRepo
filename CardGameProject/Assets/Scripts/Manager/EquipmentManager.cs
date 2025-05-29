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

    public List<WeaponData> EquippedWeapons;

    private void Awake()
    {
        if (GameManager.Instance.IsWeaponsSaved)
        {
            LoadWeapons();
        }    
    }

    public void SaveWeapons()
    {
        GameManager.Instance.EquippedWeapons = EquippedWeapons;
        GameManager.Instance.MainHand = MainHand;
        GameManager.Instance.OffHand = OffHand;

        GameManager.Instance.IsWeaponsSaved = true;
    }

    public void LoadWeapons()
    {
        EquippedWeapons = GameManager.Instance.EquippedWeapons;
        MainHand = GameManager.Instance.MainHand;
        OffHand = GameManager.Instance.OffHand;
    }
}
