using events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatStateMachine : MonoBehaviour
{
    [SerializeField] string debugState;
    [SerializeField] string subState;

    [Header("Player/Enemy")]
    public Player player;
    public List<Enemy> enemyList;

    [Header("Card References")]
    public GameObject cardPrefab;
    public Transform playerHand;
    public List<Card> cardsInDeck;

    [Header("Card Settings")]
    public int cardsToDraw = 2;

    // Variables
    [HideInInspector] public bool isPlayedCard;
    [HideInInspector] public Card cardPlayed;
    [HideInInspector] public Enemy selectedTarget;
    [HideInInspector] public bool isAttacking;

    [HideInInspector]
    public VariableScriptObject vso; // Not using for now

    public CombatBaseState currentState;
    private CombatStateFactory states;


    // Start is called before the first frame update
    void Start()
    {
        

        states = new CombatStateFactory(this, vso);
        currentState = new PlayerState(this, states, vso);
        currentState.EnterState();
    }

    // Update is called once per frame
    void Update()
    {
        currentState.UpdateStates();
        if (currentState.currentSubState != null)
        {
            subState = currentState.currentSubState.ToString();
        }
        debugState = currentState.ToString();
    }

    public void CreateCard(Card cardDrawed)
    {
        Instantiate(cardPrefab, playerHand).GetComponent<CardDisplay>().card = cardDrawed;
    }

    
    public void OnCardPlayed(CardPlayed evt, Card card)
    {
        if (player.hasEnoughStamina(card.cost))
        {
            // Destory Card
            CardContainer container = playerHand.GetComponent<CardContainer>();
            container.DestroyCard(evt.card);

            // Allow to switch to attack state
            isPlayedCard = true;
            cardPlayed = card;
        }
    }
}
