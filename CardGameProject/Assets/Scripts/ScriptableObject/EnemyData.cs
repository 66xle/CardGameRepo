using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Enemy", menuName = "Enemy")]
public class EnemyData : ScriptableObject
{
    public int Health;
    public int Guard;
    public GameObject Prefab;
    public List<Card> CardList;
}
