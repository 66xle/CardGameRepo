using MyBox;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;



public enum WeaponType
{
    Sword,
    Dagger,
    Scythe,
    Twohanded,
}

[Flags]
public enum OffSetHolster
{
    Back = 1,
    LowerBack = 2,
    RightHip = 4,
    LeftHip = 8,
    RightChest = 16,
    LeftChest = 32
}

public class Weapon : MonoBehaviour
{
    [ReadOnly] public string Guid;
    [HideInInspector] public Transform HolsterParent;

    public WeaponType weaponType;

    [Separator("Offset")]

    public OffSetHolster offSetHolster;

    [ConditionalField(nameof(offSetHolster), false, OffSetHolster.Back)] public GameObject backPrefab = null;
    [ConditionalField(nameof(offSetHolster), false, OffSetHolster.LowerBack)] public GameObject lowerBackPrefab = null;
    [ConditionalField(nameof(offSetHolster), false, OffSetHolster.RightHip)] public GameObject rightHipPrefab = null;
    [ConditionalField(nameof(offSetHolster), false, OffSetHolster.LeftHip)] public GameObject leftHipPrefab = null ;
    [ConditionalField(nameof(offSetHolster), false, OffSetHolster.RightChest)] public GameObject rightChestPrefab = null;
    [ConditionalField(nameof(offSetHolster), false, OffSetHolster.LeftChest)] public GameObject leftChestPrefab = null;
}
