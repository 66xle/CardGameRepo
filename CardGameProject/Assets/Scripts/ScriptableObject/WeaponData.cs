using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "WeaponData")]
public class WeaponData : ScriptableObject 
{

    [ReadOnly] public string Guid;

    public string WeaponName;
    public string Description;
    public DamageType DamageType;
    public WeaponType WeaponType;
    

    public GameObject Prefab;

    [HideInInspector] public Transform HolsterSlot;

    [Separator]
    public List<Card> Cards;
}
