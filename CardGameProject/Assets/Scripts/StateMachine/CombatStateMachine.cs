using events;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    

    [Header("Enemy")]
    public List<Transform> enemySpawnPosList;
    [HideInInspector] public List<Enemy> enemyList;

    [Header("Card References")]
    public GameObject cardPrefab;
    public Transform playerHand;
    public Transform displayCard;
    public List<Card> playerDeck;
    [HideInInspector] public List<Card> discardPile;
    [HideInInspector] public List<Card> enemyCardQueue;

    [Header("Card Settings")]
    public int cardsToDraw = 2;

    [Header("References")]
    public Button endTurnButton;
    public Slider healthBar;
    public TMP_Text healthValue;
    public Slider staminaBar;
    public TMP_Text staminaValue;
    public Material defMat;
    public Material redMat;

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

    public void Init(List<EnemyObj> enemyObjList)
    {
        isPlayedCard = false;
        cardPlayed = null;
        isAttacking = false;
        isPlayState = false;
        pressedEndTurnButton = false;
        enemyTurnDone = false;

        enemyList = new List<Enemy>();

        LoadPlayerAndEnemy(enemyObjList);

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

        SelectEnemy();
    }

    void LoadPlayerAndEnemy(List<EnemyObj> enemyObjList)
    {
        // Spawn Player
        player = Instantiate(playerPrefab, playerSpawnPos).GetComponent<Player>();
        player.healthBar = healthBar;
        player.healthValue = healthValue;
        player.staminaBar = staminaBar;
        player.staminaValue = staminaValue;
        player.Init();

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

    
    public void OnCardPlayed(CardPlayed evt, Card card)
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

    public void EndTurn()
    {
        pressedEndTurnButton = true;
    }

    public void DestroyEnemy(Enemy enemy)
    {
        Destroy(enemy.gameObject);
    }

    void SelectEnemy()
    {
        if (!Input.GetKeyDown(KeyCode.Mouse0) || !isPlayState)
            return;

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
}
