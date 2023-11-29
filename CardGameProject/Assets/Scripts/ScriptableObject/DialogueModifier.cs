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

    public DialogueModifier(List<Card> cards, int money, Sprite image)
    {
        this.cards = cards;
        this.money = money;
        this.image = image;
    }
}
