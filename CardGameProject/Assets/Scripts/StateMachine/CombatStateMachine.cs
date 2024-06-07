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
    public string currentSuperState;
    [SerializeField] string subState;

    [Header("Player")]
    public GameObject playerPrefab;
    public Transform playerSpawnPos;
    [HideInInspector] public Player player;
    public Slider healthBar;
    public TMP_Text healthValue;
    public Slider staminaBar;
    public TMP_Text staminaValue;
    public TMP_Text blockValue;


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

    [Header("References")]
    public InputManager inputManager;
    public EventDisplay eventDisplay;
    public CardManager cardManager;
    public Button endTurnButton;
    public Material defMat;
    public Material redMat;

    #region Internal Variables

    // Variables
    [HideInInspector] public bool isPlayedCard;
    [HideInInspector] public Card cardPlayed;
    [HideInInspector] public bool isInAction;
    [HideInInspector] public bool isPlayState;
    [HideInInspector] public bool pressedEndTurnButton;

    [HideInInspector] public bool enemyTurnDone;
    [HideInInspector] public Enemy selectedEnemy;

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
        isInAction = false;
        isPlayState = false;
        pressedEndTurnButton = false;
        enemyTurnDone = false;

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
        currentSuperState = currentState.ToString();

        if (inputManager.leftClickInputDown && isPlayState)
        {
            SelectEnemy();
        }
    }

    void LoadPlayerAndEnemy()
    {
        // Spawn Player
        player = Instantiate(playerPrefab, playerSpawnPos).GetComponent<Player>();
        player.healthBar = healthBar;
        player.healthValue = healthValue;
        player.staminaBar = staminaBar;
        player.staminaValue = staminaValue;
        player.blockValue = blockValue;
        player.Init();

        List<EnemyObj> enemyObjList = nodeData.enemies;

        // Spawn Enemy
        for (int i = 0; i < enemyObjList.Count; i++)
        {
            Enemy enemy = Instantiate(enemyObjList[i].prefab, enemySpawnPosList[i]).GetComponent<Enemy>();
            enemy.enemyObj = enemyObjList[i];
            enemy.deck = enemy.enemyObj.cardList;
            enemy.maxHealth = enemy.enemyObj.health;

            enemyList.Add(enemy);

            if (i == 0)
            {
                selectedEnemy = enemy;
                selectedEnemy.GetComponent<MeshRenderer>().material = redMat;
            }
        }
    }

    public void CreateCard(Card cardDrawed, Transform parent)
    {
        Instantiate(cardPrefab, parent).GetComponent<CardDisplay>().card = cardDrawed;
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

    public void EndTurn()
    {
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

    void SelectEnemy()
    {
        // Raycast from screen to tile
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100))
        {
            if (hit.transform.CompareTag("Enemy"))
            {
                selectedEnemy.GetComponent<MeshRenderer>().material = defMat;

                selectedEnemy = hit.transform.GetComponent<Enemy>();
                selectedEnemy.transform.GetComponent<MeshRenderer>().material = redMat;
            }
        }
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

}
