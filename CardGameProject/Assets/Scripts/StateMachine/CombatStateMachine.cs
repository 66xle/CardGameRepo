using events;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;
using MyBox;

public class CombatStateMachine : MonoBehaviour
{
    protected const string PLAYERSTATE = "PlayerState";

    public string currentSuperState; // ONLY FOR DEBUGGING DON'T USE
    [SerializeField] string subState;


    [Foldout("Player", true)]
    public GameObject playerPrefab;
    public Transform playerSpawnPos;
    [HideInInspector] public Player player;

    [Header("UI References")]
    public Slider healthBar;
    public TMP_Text healthValue;
    public Slider staminaBar;
    public TMP_Text staminaValue;
    public Slider guardBar;
    public TMP_Text guardValue;
    public TMP_Text blockValue;

    [Foldout("Enemy", true)]
    public GameObject enemyUIPrefab;
    public DetailedUI detailedUI;

    [Header("Lists")]
    public List<Transform> enemySpawnPosList;
    public List<GameObject> enemyUISpawnPosList;

    [HideInInspector] public List<Enemy> enemyList;
    [HideInInspector] public List<Enemy> enemyTurnQueue;
    [HideInInspector] public int turnIndex = 0;
    [HideInInspector] public Enemy currentEnemyTurn;

    [Foldout("Camera", true)]
    public CinemachineVirtualCamera defaultCam;
    public CinemachineVirtualCamera followCam;
    public CinemachineVirtualCamera panCam;

    [Foldout("Card", true)]
    public int cardsToDraw = 2;

    [Header("References")]
    public GameObject cardPrefab;
    public Transform playerHand;
    public Transform displayCard;
    [HideInInspector] public List<Card> discardPile;
    [HideInInspector] public List<Card> enemyCardQueue;

    [Foldout("StatusEffectObj", true)]
    public StatusEffect guardBreakLightArmour;
    public StatusEffect guardBreakMediumArmour;
    public StatusEffect guardBreakHeavyArmour;
    
    [Foldout("Animation Settings", true)]
    public float moveDuration = 0.5f;
    public float jumpDuration = 0.5f;
    public AnimationCurve moveAnimCurve;
    public AnimationCurve jumpAnimCurve;

    [Foldout("References", true)]
    public InputManager inputManager;
    public StatsManager statsManager;
    public SwitchWeaponManager switchWeaponManager;
    public CombatUIManager combatUIManager;
    public EventDisplay eventDisplay;
    public CardManager cardManager;
    public EnemyManager enemyManager;
    public GameObject rewardUI;
    public GameObject gameOverUI;
    
    

    #region Internal Variables

    // Variables
    [Foldout("Private Variables", true)]
    [ReadOnly] public bool isPlayedCard;
    [ReadOnly] public bool isPlayState;
    [ReadOnly] public bool pressedEndTurnButton;
    [ReadOnly] public bool enemyTurnDone;
    [ReadOnly] public bool skipTurn;

    [HideInInspector] public Card cardPlayed;
    [HideInInspector] public Enemy selectedEnemyToAttack;

    [HideInInspector] public VariableScriptObject vso; // Not using for now

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

        LoadPlayer();
        LoadEnemy();

        states = new CombatStateFactory(this, vso);
        currentState = new PlayerState(this, states, vso);
        currentState.EnterState();
    }

    public void Start()
    {
        isPlayedCard = false;
        cardPlayed = null;
        isPlayState = false;
        pressedEndTurnButton = false;
        enemyTurnDone = false;
        skipTurn = false;

        enemyList = new List<Enemy>();
        discardPile = new List<Card>();
        enemyCardQueue = new List<Card>();

        LoadPlayer();
        LoadEnemy();

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

        if (inputManager.leftClickInputDown && isPlayState && Time.timeScale == 1)
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
            Debug.Log("shoot ray");
            if (hit.transform.CompareTag("Enemy"))
            {
                ResetSelectedEnemyUI();
                
                selectedEnemyToAttack = hit.transform.GetComponent<Enemy>();
                selectedEnemyToAttack.EnemySelection(true);
            }
        }
    }

    private void LoadPlayer()
    {
        // Spawn Player
        player = Instantiate(playerPrefab, playerSpawnPos).GetComponent<Player>();
        player.Init(healthBar, healthValue, staminaBar, staminaValue, blockValue, guardBar, guardValue,
                    statsManager.currentMaxHealth, statsManager.currentMaxStamina, statsManager.currentMaxGuard,
                    statsManager.armourType, statsManager.damageType);

        // Equipment
        EquipmentHolster holsterScript = player.GetComponent<EquipmentHolster>();

        switchWeaponManager.InitWeaponData();

        holsterScript.SetMainHand(switchWeaponManager.currentMainHand);

        if (switchWeaponManager.IsHolstersEquipped())
            holsterScript.SetHolsteredWeapons(switchWeaponManager.currentEquippedWeapons);

        if (switchWeaponManager.IsOffhandEquipped())
            holsterScript.SetOffHand(switchWeaponManager.currentOffHand);

        followCam.Follow = player.gameObject.transform;
        panCam.Follow = player.gameObject.transform;
    }

    private void LoadEnemy()
    {
        //List<EnemyObj> enemyObjList = nodeData.enemies;
        List<EnemyObj> enemyObjList = enemyManager.enemies;


        // Spawn Enemy
        for (int i = 0; i < enemyObjList.Count; i++)
        {
            // Init Obj
            Enemy enemy = Instantiate(enemyObjList[i].prefab, enemySpawnPosList[i]).GetComponent<Enemy>();
            enemy.Init(enemyObjList[i]);
            enemyList.Add(enemy);

            // Init Stats

            if (i == 0)
                detailedUI.Init(this);

            GameObject statsUI = Instantiate(enemyUIPrefab, enemyUISpawnPosList[i].GetComponent<RectTransform>());
            enemy.InitStats(statsUI, detailedUI);

            EnemyUI enemyUI = statsUI.GetComponent<EnemyUI>();
            enemyUI.Init(this, enemy);

            // Set default selection
            if (i == 0)
            {
                selectedEnemyToAttack = enemy;
                enemy.EnemySelection(true);
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

    public void ResetSelectedEnemyUI()
    {
        for (int i = 0; i < enemyList.Count; i++)
        {
            enemyList[i].EnemySelection(false);
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

    public void AvatarDeath(Avatar currentAvatar, string deathType)
    {
        if (currentAvatar.IsAvatarDead())
        {
            currentAvatar.GetComponent<Animator>().SetTrigger("Death");

            // Check deaths
            if (deathType == "Enemy")
            {
                #region Enemies

                ResetSelectedEnemyUI();
                selectedEnemyToAttack.disableSelection = true;

                // Remove enemy
                enemyList.Remove(currentAvatar as Enemy);
                enemyTurnQueue.Remove(currentAvatar as Enemy);
                //ctx.DestroyEnemy(ctx.selectedEnemyToAttack);

                // Are there enemies still alive
                if (enemyList.Count > 0)
                {
                    // Select different enemy
                    selectedEnemyToAttack = enemyList[0];
                    selectedEnemyToAttack.EnemySelection(true);
                }
                else if (enemyList.Count == 0) // No enemies
                {
                    ClearCombatScene();

                    RewardUI();

                    //eventDisplay.FinishCombatEvent();
                }

                #endregion
            }
            else
            {
                #region Player

                // Game over UI
                GameOverUI();

                #endregion

            }
        }
    }

    public void RewardUI()
    {
        rewardUI.SetActive(true);
        Time.timeScale = 0;
    }

    public void GameOverUI()
    {
        gameOverUI.SetActive(true);
        Time.timeScale = 0;
    }

    #endregion

}
