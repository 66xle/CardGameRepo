using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueModifier : Modifier
{
    public List<Card> cards;

    [Space]

    public int money;

    [Space]

    public Sprite image;

    public DialogueModifier() { }

    public DialogueModifier(StoreModifier modifier)
    {
        cards = modifier.cards;
        money = modifier.money;
        image = modifier.image;
    }
}
