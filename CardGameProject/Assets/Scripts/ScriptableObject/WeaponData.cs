using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "WeaponData")]
public class WeaponData : ScriptableObject 
{
    [ReadOnly]
    public string guid;

    public string weaponName;
    public string description;

    public GameObject prefab;

    [HideInInspector]
    public Transform holsterSlot;

    [Separator]
    public List<Card> cards;
}
