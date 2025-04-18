using System.Collections;
using System.Collections.Generic;
using SerializeReferenceEditor;
using UnityEngine;

public enum CardType
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

    public string cardName;
    [TextArea] public string description;
    [TextArea] public string flavour;

    [Header("Card Image")]
    public Sprite image;
    public Sprite frame;

    [Header("Card Info")]
    public CardType cardType;
    public int value;
    public int cost;
    public int recycleValue;
    [SerializeReference][SR] public List<Executable> commands = new List<Executable>();
}
