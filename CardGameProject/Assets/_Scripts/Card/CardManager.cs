using MyBox;
using System.Collections.Generic;
using UnityEngine;


public class CardManager : MonoBehaviour
{
    [Header("Card")]
    public int CardsToDraw = 2;
    public Vector2 DrawOffset;
    public Vector3 SpawnScale;

    [Header("References")]
    [MustBeAssigned][SerializeField] GameObject CardPrefab;
    [MustBeAssigned] public Transform PlayerHandTransform;
    [MustBeAssigned][SerializeField] SwitchWeaponManager SwitchWeaponManager;
    [MustBeAssigned][SerializeField] EquipmentManager EquipmentManager;
    [MustBeAssigned][SerializeField] StatsManager StatsManager;
    [MustBeAssigned][SerializeField] CombatUIManager CombatUIManager;
    [MustBeAssigned][SerializeField] Camera UICamera;

    [HideInInspector] public List<CardRuntime> PlayerDeck;
    [HideInInspector] public List<CardRuntime> PlayerHand;
    [HideInInspector] public List<CardRuntime> DiscardPile;
    [HideInInspector] public List<CardRuntime> EnemyCardQueue;

    public GameObject GetCardPrefab => CardPrefab;

    void Awake()
    {
        SceneInitialize.Instance.Subscribe(Init);
    }

    private void Init()
    {
        PlayerDeck = new List<CardRuntime>();
        PlayerHand = new List<CardRuntime>();
        DiscardPile = new List<CardRuntime>();
        EnemyCardQueue = new List<CardRuntime>();
    }

    public void ResetCards()
    {
        // Shuffle deck
        if (GameManager.Instance.TutorialStage >= 5)
        {
            Extensions.Shuffle(PlayerDeck);
        }

        PlayerDeck.Clear();
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
                CardRuntime cardRuntime = CreateCardRuntime(gearData, data);
                PlayerDeck.Add(cardRuntime);
            }
        }
    }

    public CardRuntime CreateCardRuntime(GearData gearData, CardAnimationData data)
    {
        CardRuntime cardRuntime = new(gearData, data, StatsManager.Attack, StatsManager.Defence + EquipmentManager.GetArmoursDefence(), StatsManager.BlockScale, StatsManager.CurrentMaxHealth);
        return cardRuntime;
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
            string description = display.CardRuntime.GenerateDescriptionWithDamage(display.Card, display.CardRuntime.Gear, StatsManager.Attack, StatsManager.Defence, StatsManager.BlockScale, StatsManager.CurrentMaxHealth, enemy, player);
            display.UpdateDescription(description);
            display.SetCamera(UICamera);
        }
    }

    public void CreateCard(CardRuntime cardRuntime, Transform parent)
    {
        GameObject cardObject = Instantiate(CardPrefab, parent);

        // Spawn location
        RectTransform rect = cardObject.GetComponent<RectTransform>();
        rect.localPosition += new Vector3(DrawOffset.x, DrawOffset.y, rect.localPosition.z);
        rect.localScale = SpawnScale;

        CardDisplay cardDisplay = cardObject.GetComponent<CardDisplay>();
        cardDisplay.SetCard(cardRuntime, cardRuntime.Card);
    }

    public void SetCardDisplay(GameObject cardObject, CardRuntime data)
    {
        // Set card speed to zero so it snaps to new position instead of flying there
        StartCoroutine(cardObject.GetComponent<CardWrapper>().CardSpeedToZero());

        CardDisplay cardDisplay = cardObject.GetComponent<CardDisplay>();
        cardDisplay.SetCard(data, data.Card);
    }

    public void DrawCards(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            if (PlayerDeck.Count <= 0)
            {
                // Reset deck and clear discard pile
                PlayerDeck = new List<CardRuntime>(DiscardPile);
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
            CardRuntime cardDrawed;

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
