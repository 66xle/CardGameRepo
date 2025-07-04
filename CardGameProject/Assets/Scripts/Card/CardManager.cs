using System;
using System.Collections.Generic;
using System.Linq;
using MyBox;
using UnityEngine;


public class CardManager : MonoBehaviour
{
    [Header("Card")]
    public int CardsToDraw = 2;

    [Header("References")]
    [MustBeAssigned] public GameObject CardPrefab;
    [MustBeAssigned] public Transform PlayerHandTransform;
    [MustBeAssigned] public SwitchWeaponManager SwitchWeaponManager;


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
        PlayerDeck.AddRange(SwitchWeaponManager.CurrentMainHand.Cards.Select(card => new CardData(SwitchWeaponManager.CurrentMainHand, card)));

        foreach (WeaponData weaponData in SwitchWeaponManager.CurrentEquippedWeapons)
        {
            PlayerDeck.AddRange(weaponData.Cards.Select(card => new CardData(weaponData, card)));
        }
    }
}
