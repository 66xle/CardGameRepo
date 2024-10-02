using events;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using TMPro;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;
using Cinemachine;

public class CombatStateMachine : MonoBehaviour
{
    protected const string PLAYERSTATE = "PlayerState";

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
    public List<GameObject> enemyUISpawnPosList;
    public GameObject enemyUIPrefab;
    public DetailedUI detailedUI;
    [HideInInspector] public List<Enemy> enemyList;
    [HideInInspector] public List<Enemy> enemyTurnQueue;
    [HideInInspector] public int turnIndex = 0;
    [HideInInspector] public Enemy currentEnemyTurn;

    [Header("Camera References")]
    public CinemachineVirtualCamera defaultCam;
    public CinemachineVirtualCamera followCam;
    public CinemachineVirtualCamera panCam;

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

    [Header("Animation Settings")]
    public float moveDuration = 0.5f;
    public float jumpDuration = 0.5f;
    public AnimationCurve moveAnimCurve;
    public AnimationCurve jumpAnimCurve;

    [Header("References")]
    public GameObject UI;
    public InputManager inputManager;
    public StatsManager statsManager;
    public EquipmentManager equipmentManager;
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
            Debug.Log("shoot ray");
            if (hit.transform.CompareTag("Enemy"))
            {
                ResetSelectedEnemyUI();
                
                selectedEnemyToAttack = hit.transform.GetComponent<Enemy>();
                selectedEnemyToAttack.EnemySelection(true);
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

        // Equipment
        EquipmentHolster holsterScript = player.GetComponent<EquipmentHolster>();
        holsterScript.SetMainHand(equipmentManager.mainHand);
        holsterScript.SetHolsteredWeapons(equipmentManager.equippedWeapons);

        if (equipmentManager.offHand != null)
        {
            holsterScript.SetOffHand(equipmentManager.offHand);
        }

        followCam.Follow = player.gameObject.transform;
        panCam.Follow = player.gameObject.transform;

        List<EnemyObj> enemyObjList = nodeData.enemies;

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

    public void SwitchWeapon()
    {
        equipmentManager.SetEquipmentCameraPosition(equipmentManager.lowerBackCam);

        equipmentManager.equipmentCam.Priority = 50;

        UI.SetActive(false);
    }

    public void NextWeapon()
    {
        equipmentManager.TransitionToNextWeapon(equipmentManager.rightHipCam);
    }

    public void PrevWeapon()
    {
        equipmentManager.TransitionToNextWeapon(equipmentManager.lowerBackCam);
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

                    eventDisplay.FinishCombatEvent();
                }

                #endregion
            }
            else
            {
                #region Player

                // Game over UI

                #endregion

            }
        }
    }

    #endregion

}
