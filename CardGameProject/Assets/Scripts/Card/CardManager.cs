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
    [MustBeAssigned] [SerializeField] StatsManager StatsManager;

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
        // Load main hand
        foreach (WeaponCardData data in SwitchWeaponManager.CurrentMainHand._cards)
        {
            for (int i = 0; i < data.CardAmount; i++)
            {
                CardData cardData = new(SwitchWeaponManager.CurrentMainHand, data, StatsManager.Attack, StatsManager.Defence, StatsManager.BlockScale);
                PlayerDeck.Add(cardData);
            }
        }
    
        // Load holstered cards
        foreach (WeaponData weaponData in SwitchWeaponManager.CurrentEquippedWeapons)
        {
            foreach (WeaponCardData data in weaponData._cards)
            {
                for (int i = 0; i < data.CardAmount; i++)
                {
                    CardData cardData = new(weaponData, data, StatsManager.Attack, StatsManager.Defence, StatsManager.BlockScale);
                    PlayerDeck.Add(cardData);
                }
            }
        }
    }

    public void UpdateCardsInHand(Enemy enemy)
    {
        for (int i = 0; i < PlayerHandTransform.childCount; i++)
        {
            GameObject go = PlayerHandTransform.GetChild(i).gameObject;
            CardDisplay display = go.GetComponent<CardDisplay>();
            string description = display.CardData.GenerateDescriptionWithDamage(display.Card, display.CardData.Weapon, StatsManager.Attack, StatsManager.Defence, StatsManager.BlockScale, enemy);
            display.UpdateDescription(description);
        }
    }
}
