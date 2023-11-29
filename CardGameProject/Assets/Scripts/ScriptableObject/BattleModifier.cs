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

    public BattleModifier(List<EnemyObj> enemies, List<Card> cards, int money)
    {
        this.enemies = enemies;
        this.cards = cards;
        this.money = money;
    }
}
