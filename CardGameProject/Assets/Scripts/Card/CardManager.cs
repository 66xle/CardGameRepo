using System;
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

        Card copyCard = new Card();
        copyCard.guid = card.guid;
        copyCard.InGameGUID = Guid.NewGuid().ToString();
        copyCard.cardName = card.cardName;
        copyCard.description = card.description;
        copyCard.flavour = card.flavour;
        copyCard.effectOption = card.effectOption;
        copyCard.value = card.value;
        copyCard.cost = card.cost;
        copyCard.recycleValue = card.recycleValue;
        copyCard.image = card.image;
        copyCard.frame = card.frame;
        copyCard.cardName = card.cardName;
        copyCard.commands = card.commands;

        this.card = copyCard;
    }
}

public class CardManager : MonoBehaviour
{
    public SwitchWeaponManager switchWeaponManager;

    [HideInInspector] public List<CardData> playerDeck;
    [HideInInspector] public List<CardData> playerHand;
    [HideInInspector] public List<CardData> discardPile;
    [HideInInspector] public List<CardData> enemyCardQueue;

    void Awake()
    {
        playerDeck = new List<CardData>();
        playerHand = new List<CardData>();
        discardPile = new List<CardData>();
        enemyCardQueue = new List<CardData>();
    }

    public void LoadCards()
    {
        foreach (WeaponData weaponData in switchWeaponManager.currentEquippedWeapons)
        {
            playerDeck.AddRange(weaponData.cards.Select(card => new CardData(weaponData, card)));
        }
    }
}
