using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;


public class CardManager : MonoBehaviour
{
    public SwitchWeaponManager SwitchWeaponManager;

    [HideInInspector] public List<CardData> PlayerDeck;
    [HideInInspector] public List<CardData> PlayerHand;
    [HideInInspector] public List<CardData> DiscardPile;
    [HideInInspector] public List<CardData> EnemyCardQueue;

    void Awake()
    {
        PlayerDeck = new List<CardData>();
        PlayerHand = new List<CardData>();
        DiscardPile = new List<CardData>();
        EnemyCardQueue = new List<CardData>();
    }

    public void LoadCards()
    {
        PlayerDeck.AddRange(SwitchWeaponManager.currentMainHand.cards.Select(card => new CardData(SwitchWeaponManager.currentMainHand, card)));

        foreach (WeaponData weaponData in SwitchWeaponManager.currentEquippedWeapons)
        {
            PlayerDeck.AddRange(weaponData.cards.Select(card => new CardData(weaponData, card)));
        }
    }
}
