using System.Collections;
using System.Collections.Generic;
using SerializeReferenceEditor;
using UnityEngine;
using MyBox;

public enum EffectOption
{
    None,
    Overwrite,
    //Stack
}

[CreateAssetMenu(fileName = "New Card", menuName = "Card")]
public class Card : ScriptableObject
{
    [ReadOnly] public string guid;
    [ReadOnly] public string InGameGUID;

    public string cardName;
    [TextArea] public string description;
    [TextArea] public string flavour;

    [Header("Card Image")]
    public Sprite image;
    public Sprite frame;

    [Header("Card Info")]
    public EffectOption effectOption;
    public int value;
    public int cost;
    public int recycleValue;
    [SerializeReference][SR] public List<Executable> commands = new List<Executable>();
}
