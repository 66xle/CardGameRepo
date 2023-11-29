using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoreModifier
{
    public List<EnemyObj> enemies;
    public List<Card> cards;
    public int money;
    public Sprite image;

    public StoreModifier() { }

    public StoreModifier(BattleModifier batMod)
    {
        enemies = batMod.enemies;
        cards = batMod.cards;
        money = batMod.money;
    }

    public StoreModifier(DialogueModifier diaMod)
    {
        cards = diaMod.cards;
        money = diaMod.money;
        image = diaMod.image;
    }
}
