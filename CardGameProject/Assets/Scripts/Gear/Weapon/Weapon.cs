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

    public WeaponType WeaponType;

    [Separator("Offset")]

    public OffSetHolster OffSetHolster;

    [ConditionalField(nameof(OffSetHolster), false, OffSetHolster.Back)] public GameObject BackPrefab = null;
    [ConditionalField(nameof(OffSetHolster), false, OffSetHolster.LowerBack)] public GameObject LowerBackPrefab = null;
    [ConditionalField(nameof(OffSetHolster), false, OffSetHolster.RightHip)] public GameObject RightHipPrefab = null;
    [ConditionalField(nameof(OffSetHolster), false, OffSetHolster.LeftHip)] public GameObject LeftHipPrefab = null ;
    [ConditionalField(nameof(OffSetHolster), false, OffSetHolster.RightChest)] public GameObject RightChestPrefab = null;
    [ConditionalField(nameof(OffSetHolster), false, OffSetHolster.LeftChest)] public GameObject LeftChestPrefab = null;

    [Separator("Render Preview")]

    public Vector3 positionOffset;
    public Vector3 rotationOffset;
}
