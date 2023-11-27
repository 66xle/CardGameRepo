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

    [Header("Player/Enemy")]
    public GameObject playerPrefab;
    [HideInInspector] public Player player;
    public Transform playerPosition;
    [HideInInspector] public List<EnemyObj> enemyList;
    public List<Transform> enemyPositions;

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


    // Start is called before the first frame update
    void Start()
    {
        isPlayedCard = false;
        cardPlayed = null;
        isAttacking = false;
        isPlayState = false;
        pressedEndTurnButton = false;
        enemyTurnDone = false;

        enemyList = new List<EnemyObj>();

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



        SelectEnemy();
    }

    void LoadPlayerAndEnemy()
    {
        // Spawn Player
        player = Instantiate(playerPrefab, playerPosition).GetComponent<Player>();
        player.healthBar = healthBar;
        player.healthValue = healthValue;
        player.staminaBar = staminaBar;
        player.staminaValue = staminaValue;
        player.Init();

        // Spawn Enemy
        for (int i = 0; i < enemyList.Count; i++)
        {
            Enemy enemy = Instantiate(enemyList[i].prefab, enemyPositions[i]).GetComponent<Enemy>();
            enemy.enemyObj = enemyList[i];
            enemy.deck = enemy.enemyObj.cardList;
        }

        selectedEnemy = enemyList[0].prefab.GetComponent<Enemy>();
        selectedEnemy.GetComponent<MeshRenderer>().material = redMat;
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
