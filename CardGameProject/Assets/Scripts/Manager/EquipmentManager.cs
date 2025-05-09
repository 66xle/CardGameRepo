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
}
