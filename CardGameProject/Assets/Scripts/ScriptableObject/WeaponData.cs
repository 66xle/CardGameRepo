using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "WeaponData")]
public class WeaponData : ScriptableObject 
{
    public string weaponName;
    public string description;

    public GameObject prefab;

    [ReadOnly]
    public Transform holsterSlot;

    [Separator]
    public List<Card> cards;
}
