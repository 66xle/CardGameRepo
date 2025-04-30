using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Enemy", menuName = "Enemy")]
public class EnemyObj : ScriptableObject
{
    public int health;
    public int guard;
    public GameObject prefab;
    public List<Card> cardList;
}
