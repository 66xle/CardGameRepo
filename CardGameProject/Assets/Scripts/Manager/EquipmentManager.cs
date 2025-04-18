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
    public WeaponData mainHand;
    public WeaponData offHand;


    [Separator]

    public List<WeaponData> equippedWeapons;
}
