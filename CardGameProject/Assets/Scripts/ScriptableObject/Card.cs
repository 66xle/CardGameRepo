using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Type
{
    Attack,
    Counter,
    Defend,
    Heal
}

[CreateAssetMenu(fileName = "New Card", menuName = "Card")]
public class Card : ScriptableObject
{
    [HideInInspector]
    public string guid;

    public string name;
    [TextArea] public string description;
    [TextArea] public string flavour;

    [Header("Card Image")]
    public Sprite image;
    public Sprite frame;

    [Header("Card Info")]
    public Type cardType;
    public int value;
    public int cost;
    public int recycleValue;

    [Header("Status Effects")]
    public List<StatusEffect> selfEffects;
    public List<StatusEffect> applyEffects;
}
