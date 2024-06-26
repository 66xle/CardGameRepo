using events;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CombatStateMachine : MonoBehaviour
{
    public string currentSuperState; // ONLY FOR DEBUGGING DON'T USE
    [SerializeField] string subState;

    [Header("Player")]
    public GameObject playerPrefab;
    public Transform playerSpawnPos;
    [HideInInspector] public Player player;

    [Header("Player References")]
    public Slider healthBar;
    public TMP_Text healthValue;
    public Slider staminaBar;
    public TMP_Text staminaValue;
    public TMP_Text blockValue;
    public Slider guardBar;
    public TMP_Text guardValue;


    [Header("Enemy")]
    public List<Transform> enemySpawnPosList;
    [HideInInspector] public List<Enemy> enemyList;
    [HideInInspector] public List<Enemy> enemyTurnQueue;
    [HideInInspector] public int turnIndex = 0;
    [HideInInspector] public Enemy currentEnemyTurn;

    [Header("Card References")]
    public GameObject cardPrefab;
    public Transform playerHand;
    public Transform displayCard;
    [HideInInspector] public List<Card> discardPile;
    [HideInInspector] public List<Card> enemyCardQueue;

    [Header("Card Settings")]
    public int cardsToDraw = 2;

    [Header("StatusEffectObj")]
    public StatusEffect guardBreakLightArmour;
    public StatusEffect guardBreakMediumArmour;
    public StatusEffect guardBreakHeavyArmour;
    [HideInInspector] public bool skipTurn;

    [Header("References")]
    public InputManager inputManager;
    public StatsManager statsManager;
    public EventDisplay eventDisplay;
    public CardManager cardManager;
    public Button endTurnButton;
    public Material defMat;
    public Material redMat;

    #region Internal Variables

    // Variables
    [HideInInspector] public bool isPlayedCard;
    [HideInInspector] public Card cardPlayed;
    [HideInInspector] public bool isPlayState;
    [HideInInspector] public bool pressedEndTurnButton;

    [HideInInspector] public bool enemyTurnDone;
    [HideInInspector] public Enemy selectedEnemyToAttack;

    [HideInInspector]
    public VariableScriptObject vso; // Not using for now

    public CombatBaseState currentState;
    private CombatStateFactory states;
    private DialogueNodeData nodeData;

    #endregion

    public void Init(DialogueNodeData nodeData)
    {
        this.nodeData = nodeData;

        isPlayedCard = false;
        cardPlayed = null;
        isPlayState = false;
        pressedEndTurnButton = false;
        enemyTurnDone = false;
        skipTurn = false;

        enemyList = new List<Enemy>();
        discardPile = new List<Card>();
        enemyCardQueue = new List<Card>();

        LoadPlayerAndEnemy();

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

        if (inputManager.leftClickInputDown && isPlayState)
        {
            SelectEnemy();
        }
    }

    private void SelectEnemy()
    {
        // Raycast from screen to tile
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100))
        {
            if (hit.transform.CompareTag("Enemy"))
            {
                selectedEnemyToAttack.GetComponent<MeshRenderer>().material = defMat;

                selectedEnemyToAttack = hit.transform.GetComponent<Enemy>();
                selectedEnemyToAttack.transform.GetComponent<MeshRenderer>().material = redMat;
            }
        }
    }

    private void LoadPlayerAndEnemy()
    {
        // Spawn Player
        player = Instantiate(playerPrefab, playerSpawnPos).GetComponent<Player>();
        player.Init(healthBar, healthValue, staminaBar, staminaValue, blockValue, guardBar, guardValue,
                    statsManager.currentMaxHealth, statsManager.currentMaxStamina, statsManager.currentMaxGuard,
                    statsManager.armourType, statsManager.damageType);

        List<EnemyObj> enemyObjList = nodeData.enemies;

        // Spawn Enemy
        for (int i = 0; i < enemyObjList.Count; i++)
        {
            Enemy enemy = Instantiate(enemyObjList[i].prefab, enemySpawnPosList[i]).GetComponent<Enemy>();
            enemy.enemyObj = enemyObjList[i];
            enemy.deck = enemy.enemyObj.cardList;
            enemy.maxHealth = enemy.enemyObj.health;

            enemyList.Add(enemy);

            // Set default selection
            if (i == 0)
            {
                selectedEnemyToAttack = enemy;
                selectedEnemyToAttack.GetComponent<MeshRenderer>().material = redMat;
            }
        }
    }

    public void OnCardPlayed(CardPlayed evt, Card card, string tag)
    {
        if (tag == "Play")
        {
            if (player.hasEnoughStamina(card.cost))
            {
                player.ConsumeStamina(card.cost);

                discardPile.Add(card);

                // Destory Card
                CardContainer container = playerHand.GetComponent<CardContainer>();
                container.DestroyCard(evt.card);

                // Allow to switch to attack state
                isPlayedCard = true;
                cardPlayed = card;
            }
        }
        else if (tag == "Recycle")
        {
            // Destory Card
            CardContainer container = playerHand.GetComponent<CardContainer>();
            container.DestroyCard(evt.card);

            player.RecycleCardToStamina(card.recycleValue);
        }
    }

    #region Used by StateMachine

    public void CreateCard(Card cardDrawed, Transform parent)
    {
        Instantiate(cardPrefab, parent).GetComponent<CardDisplay>().card = cardDrawed;
    }

    public void EndTurn()
    {
        Debug.Log("END PLAYER'S TURN");

        pressedEndTurnButton = true;

        // For enemy state
        enemyTurnQueue.Clear();
        enemyTurnQueue = Extensions.Clone(enemyList);

        enemyTurnDone = false;
    }

    public void DestroyEnemy(Enemy enemy)
    {
        Destroy(enemy.gameObject);
    }

    public void ClearCombatScene()
    {
        // Destroy card in hand
        foreach (Transform child in playerHand)
        {
            Destroy(child.gameObject);
        }

        // destroy player
        Destroy(playerSpawnPos.GetChild(0).gameObject);

        // destroy enemies
        foreach (Transform pos in enemySpawnPosList)
        {
            if (enemyList.Count == 0)
                break;

            Destroy(pos.GetChild(0).gameObject);
        }
    }

    #endregion

}
