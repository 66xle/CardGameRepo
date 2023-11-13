using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Type
{
    Attack,
    Defend,
    Heal
}

[CreateAssetMenu(fileName = "New Card", menuName = "Card")]
public class Card : ScriptableObject
{
    public string name;
    public string description;

    public Sprite image;

    public Type cardType;
    public int value;
}
