using System.Collections.Generic;
using MyBox;
using UnityEngine;


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
    [MustBeAssigned] [SerializeField] CombatUIManager CombatUIManager;
    [MustBeAssigned] [SerializeField] Camera UICamera;

    [HideInInspector] public List<CardData> PlayerDeck;
    [HideInInspector] public List<CardData> PlayerHand;
    [HideInInspector] public List<CardData> DiscardPile;
    [HideInInspector] public List<CardData> EnemyCardQueue;

    void Awake()
    {
        SceneInitialize.Instance.Subscribe(Init);
    }

    private void Init()
    {
        PlayerDeck = new List<CardData>();
        PlayerHand = new List<CardData>();
        DiscardPile = new List<CardData>();
        EnemyCardQueue = new List<CardData>();
    }

    public void ResetCards()
    {
        PlayerDeck.AddRange(PlayerHand);
        PlayerDeck.AddRange(DiscardPile);

        PlayerHand.Clear();
        DiscardPile.Clear();

        CardContainer container = PlayerHandTransform.GetComponent<CardContainer>();
        container.DestroyAllCards();
    }

    public void AddEquipmentCardsToDeck(GearData gearData)
    {
        foreach (CardAnimationData data in gearData.Cards)
        {
            for (int i = 0; i < data.CardAmount; i++)
            {
                CardData cardData = new(gearData, data, StatsManager.Attack, StatsManager.Defence + EquipmentManager.GetArmoursDefence(), StatsManager.BlockScale, StatsManager.CurrentMaxHealth);
                PlayerDeck.Add(cardData);
            }
        }
    }

    public void LoadCards()
    {
        AddEquipmentCardsToDeck(SwitchWeaponManager.CurrentMainHand);

        return;

        // Load main hand
        AddEquipmentCardsToDeck(SwitchWeaponManager.CurrentMainHand);

        foreach (WeaponData weaponData in SwitchWeaponManager.CurrentEquippedWeapons)
        {
            AddEquipmentCardsToDeck(weaponData);
        }

        foreach (ArmourData armour in EquipmentManager.GetEquippedArmours())
        {
            AddEquipmentCardsToDeck(armour);
        }
    }

    public void UpdateCardsInHand(Avatar enemy, Avatar player)
    {
        for (int i = 0; i < PlayerHandTransform.childCount; i++)
        {
            GameObject go = PlayerHandTransform.GetChild(i).gameObject;
            CardDisplay display = go.GetComponent<CardDisplay>();
            string description = display.CardData.GenerateDescriptionWithDamage(display.Card, display.CardData.Gear, StatsManager.Attack, StatsManager.Defence, StatsManager.BlockScale, StatsManager.CurrentMaxHealth, enemy, player);
            display.UpdateDescription(description);
            display.SetCamera(UICamera);
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
                if (!GameManager.Instance.IsInTutorial)
                {
                    Extensions.Shuffle(PlayerDeck);
                }
            }

            // No more cards to draw
            if (PlayerDeck.Count <= 0)
                break;

            // Pick random card
            CardData cardDrawed;

            // Shuffle deck
            if (GameManager.Instance.IsInTutorial && GameManager.Instance.TutorialStage < 5f)
            {
                cardDrawed = PlayerDeck[0];
            }
            else
            {
                int index = Random.Range(0, PlayerDeck.Count);
                cardDrawed = PlayerDeck[index];
            }

            

            CreateCard(cardDrawed, PlayerHandTransform);
            PlayerDeck.Remove(cardDrawed);
            PlayerHand.Add(cardDrawed);
        }
    }
}
