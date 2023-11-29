using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleModifier : Modifier
{
    public List<EnemyObj> enemies;

    [Space]
    [Space]

    public List<Card> cards;

    [Space]

    public int money;

    public BattleModifier() { }

    public BattleModifier(StoreModifier modifier)
    {
        enemies = modifier.enemies;
        cards = modifier.cards;
        money = modifier.money;
    }
}
