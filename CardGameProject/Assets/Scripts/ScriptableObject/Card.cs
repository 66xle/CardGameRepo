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
    [TextArea] public string description;
    [TextArea] public string flavour;

    public Sprite image;
    public Sprite frame;

    public Type cardType;
    public int value;
    public int cost;
    public int recycleValue;
}
