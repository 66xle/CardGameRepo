using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public struct CardData
{
    public WeaponData weapon;
    public Card card;

    public CardData(WeaponData weapon, Card card)
    {
        this.weapon = weapon;
        this.card = card;
    }
}

public class CardManager : MonoBehaviour
{
    public EquipmentManager equipmentManager;

    [HideInInspector] public List<CardData> playerDeck;
    [HideInInspector] public List<CardData> discardPile;
    [HideInInspector] public List<CardData> enemyCardQueue;

    void Awake()
    {
        playerDeck = new List<CardData>();
        discardPile = new List<CardData>();
        enemyCardQueue = new List<CardData>();
        LoadCards();
    }

    public void LoadCards()
    {
        foreach (WeaponData weaponData in equipmentManager.equippedWeapons)
        {
            playerDeck.AddRange(weaponData.cards.Select(card => new CardData(weaponData, card)));
        }
    }
}
