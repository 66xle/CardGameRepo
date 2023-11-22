using events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CombatStateMachine : MonoBehaviour
{
    public string currentSuperState;
    [SerializeField] string subState;

    [Header("Player/Enemy")]
    public Player player;
    public List<Enemy> enemyList;
    [HideInInspector] public List<Card> enemyCardQueue;

    [Header("Card References")]
    public GameObject cardPrefab;
    public Transform playerHand;
    public List<Card> cardsInDeck;
    public Transform displayCard;

    [Header("Card Settings")]
    public int cardsToDraw = 2;

    [Header("References")]
    public Button endTurnButton;

    // Variables
    [HideInInspector] public bool isPlayedCard;
    [HideInInspector] public Card cardPlayed;
    [HideInInspector] public bool isAttacking;
    [HideInInspector] public bool isPlayState;
    [HideInInspector] public bool pressedEndTurnButton;
    [HideInInspector] public bool enemyTurnDone;

    [HideInInspector] public Enemy selectedEnemy;

    [HideInInspector]
    public VariableScriptObject vso; // Not using for now

    public CombatBaseState currentState;
    private CombatStateFactory states;


    // Start is called before the first frame update
    void Start()
    {
        isPlayedCard = false;
        cardPlayed = null;
        isAttacking = false;
        isPlayState = false;
        pressedEndTurnButton = false;
        enemyTurnDone = false;

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
        currentSuperState = currentState.ToString();
    }

    public void CreateCard(Card cardDrawed, Transform parent)
    {
        Instantiate(cardPrefab, parent).GetComponent<CardDisplay>().card = cardDrawed;
    }

    
    public void OnCardPlayed(CardPlayed evt, Card card)
    {
        if (player.hasEnoughStamina(card.cost))
        {
            player.ConsumeStamina(card.cost);

            // Destory Card
            CardContainer container = playerHand.GetComponent<CardContainer>();
            container.DestroyCard(evt.card);

            // Allow to switch to attack state
            isPlayedCard = true;
            cardPlayed = card;
        }
    }

    public void EndTurn()
    {
        pressedEndTurnButton = true;
    }

    public void DestroyEnemy(Enemy enemy)
    {
        Destroy(enemy.gameObject);
    }
}
