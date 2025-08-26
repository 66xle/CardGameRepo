using System.Collections.Generic;
using System.Linq;
using MyBox;
using Unity.Android.Gradle.Manifest;
using UnityEngine;
using System.Collections;


public class CardManager : MonoBehaviour
{
    [Header("Card")]
    public int CardsToDraw = 2;

    [Header("References")]
    [MustBeAssigned] [SerializeField] GameObject CardPrefab; 
    [MustBeAssigned] public Transform PlayerHandTransform;
    [MustBeAssigned] [SerializeField] SwitchWeaponManager SwitchWeaponManager;
    [MustBeAssigned] [SerializeField] EquipmentManager EquipmentManager;
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
        foreach (CardAnimationData data in SwitchWeaponManager.CurrentMainHand._cards)
        {
            for (int i = 0; i < data.CardAmount; i++)
            {
                CardData cardData = new(SwitchWeaponManager.CurrentMainHand, data, StatsManager.Attack, StatsManager.Defence + EquipmentManager.GetArmoursDefence(), StatsManager.BlockScale, StatsManager.CurrentMaxHealth);
                PlayerDeck.Add(cardData);
            }
        }
    
        // Load holstered cards
        foreach (WeaponData weaponData in SwitchWeaponManager.CurrentEquippedWeapons)
        {
            foreach (CardAnimationData data in weaponData._cards)
            {
                for (int i = 0; i < data.CardAmount; i++)
                {
                    CardData cardData = new(weaponData, data, StatsManager.Attack, StatsManager.Defence + EquipmentManager.GetArmoursDefence(), StatsManager.BlockScale, StatsManager.CurrentMaxHealth);
                    PlayerDeck.Add(cardData);
                }
            }
        }

        foreach (ArmourData armour in EquipmentManager.GetEquippedArmours())
        {
            foreach (CardAnimationData card in armour._cards)
            {
                for (int i = 0; i < card.CardAmount; i++)
                {
                    CardData cardData = new(armour, card, StatsManager.Attack, StatsManager.Defence + EquipmentManager.GetArmoursDefence(), StatsManager.BlockScale, StatsManager.CurrentMaxHealth);
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
            string description = display.CardData.GenerateDescriptionWithDamage(display.Card, display.CardData.Gear, StatsManager.Attack, StatsManager.Defence, StatsManager.BlockScale, StatsManager.CurrentMaxHealth, enemy);
            display.UpdateDescription(description);
        }
    }

    public void CreateCard(CardData cardDrawed, Transform parent)
    {
        CardDisplay cardDisplay = Instantiate(CardPrefab, parent).GetComponent<CardDisplay>();
        cardDisplay.SetCard(cardDrawed, cardDrawed.Card);
    }

    public void DrawCards(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            if (PlayerDeck.Count <= 0)
            {
                // Reset deck and clear discard pile
                PlayerDeck = new List<CardData>(DiscardPile);
                DiscardPile.Clear();

                // Shuffle deck
                Extensions.Shuffle(PlayerDeck);
            }

            // No more cards to draw
            if (PlayerDeck.Count <= 0)
                break;

            // Pick random card
            int index = Random.Range(0, PlayerDeck.Count);
            CardData cardDrawed = PlayerDeck[index];

            CreateCard(cardDrawed, PlayerHandTransform);
            PlayerDeck.Remove(cardDrawed);
            PlayerHand.Add(cardDrawed);
        }
    }
}
