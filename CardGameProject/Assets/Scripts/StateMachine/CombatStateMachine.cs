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
using DG.Tweening;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;
using UnityEngine.Analytics;

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
    public float statusEffectDelay = 0.5f;
    public float statusEffectAfterDelay = 1f;
    public CinemachineVirtualCamera defaultCam;
    public CinemachineVirtualCamera followCam;
    public CinemachineVirtualCamera panCam;

    [Foldout("Card", true)]
    public int cardsToDraw = 2;

    [Header("References")]
    public GameObject cardPrefab;
    public Transform playerHandTransform;
    public Transform displayCard;


    [Foldout("StatusEffect", true)]
    public StatusEffectData guardBreakLightArmourData;
    public StatusEffectData guardBreakMediumArmourData;
    public StatusEffectData guardBreakHeavyArmourData;
    
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

    [HideInInspector] public CardData cardPlayed;
    [HideInInspector] public Enemy selectedEnemyToAttack;

    [HideInInspector] public VariableScriptObject vso; // Not using for now

    private EquipmentHolster _equipmentHolsterScript;


    public CombatBaseState currentState;
    private CombatStateFactory states;
    private DialogueNodeData nodeData;

    #endregion



    public void Start()
    {
        isPlayedCard = false;
        isPlayState = false;
        pressedEndTurnButton = false;
        enemyTurnDone = false;

        enemyList = new List<Enemy>();

        LoadPlayer();
        LoadEnemy();
        cardManager.LoadCards();



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

        if (inputManager.LeftClickInputDown && isPlayState && Time.timeScale == 1)
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
                Enemy enemy = hit.transform.GetComponent<Enemy>();

                if (enemy.IsAvatarDead()) return;

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
        player.Init(healthBar, healthValue, staminaBar, staminaValue, blockValue, guardBar, guardValue, statsManager.armourType);
        player.InitStats(statsManager.CurrentMaxHealth, statsManager.CurrentMaxStamina, statsManager.CurrentMaxGuard);

        // Equipment
        _equipmentHolsterScript = player.GetComponent<EquipmentHolster>();

        switchWeaponManager.InitWeaponData();

        List<WeaponData> holsterWeapons = new List<WeaponData>();
        holsterWeapons.Add(switchWeaponManager.CurrentMainHand);

        if (switchWeaponManager.IsOffhandEquipped())
            holsterWeapons.Add(switchWeaponManager.CurrentOffHand);

        holsterWeapons.AddRange(switchWeaponManager.CurrentEquippedWeapons);

        if (holsterWeapons.Count > 0)
        {
            _equipmentHolsterScript.SetHolsteredWeapons(holsterWeapons);

            GameObject weaponToEquip = _equipmentHolsterScript.EquippedWeaponObjects.First(weapon => weapon.GetComponent<Weapon>().Guid == switchWeaponManager.CurrentMainHand.Guid);
            _equipmentHolsterScript.EquipWeapon(weaponToEquip);
        }

        followCam.Follow = player.gameObject.transform;
        panCam.Follow = player.gameObject.transform;
    }

    private void LoadEnemy()
    {
        //List<EnemyObj> enemyObjList = nodeData.enemies;
        List<EnemyData> enemyDataList = enemyManager.Enemies;


        // Spawn Enemy
        for (int i = 0; i < enemyDataList.Count; i++)
        {
            // Init Obj
            Enemy enemy = Instantiate(enemyDataList[i].Prefab, enemySpawnPosList[i]).GetComponent<Enemy>();
            enemy.Init(enemyDataList[i]);
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
            

            if (player.hasEnoughStamina(card.Cost))
            {
                player.ConsumeStamina(card.Cost);

                // Get cardData from player hand to move to discard pile
                CardData cardData = cardManager.PlayerHand.First(data => data.Card.InGameGUID == card.InGameGUID);
                cardManager.PlayerHand.Remove(cardData);
                cardManager.DiscardPile.Add(cardData);

                // Destory Card
                CardContainer container = playerHandTransform.GetComponent<CardContainer>();
                container.DestroyCard(evt.card);


                GameObject mainHandWeapon = _equipmentHolsterScript.RightHand.GetChild(0).gameObject;
                Weapon weaponScript = mainHandWeapon.GetComponent<Weapon>();

                // Swap Weapon
                if (weaponScript.Guid != cardData.Weapon.Guid)
                {
                    GameObject holsteredWeapon = _equipmentHolsterScript.EquippedWeaponObjects.First(weapon => weapon.gameObject.GetComponent<Weapon>().Guid == cardData.Weapon.Guid);

                    _equipmentHolsterScript.HolsterWeapon(mainHandWeapon);   

                    _equipmentHolsterScript.EquipWeapon(holsteredWeapon);
                }


                // Allow to switch to attack state
                isPlayedCard = true;
                cardPlayed = cardData;
            }
        }
        else if (tag == "Recycle")
        {
            // Get cardData from player hand to move to discard pile
            CardData cardData = cardManager.PlayerHand.First(data => data.Card.InGameGUID == card.InGameGUID);
            cardManager.PlayerHand.Remove(cardData);
            cardManager.DiscardPile.Add(cardData);

            // Destory Card
            CardContainer container = playerHandTransform.GetComponent<CardContainer>();
            container.DestroyCard(evt.card);

            player.RecycleCardToStamina(card.RecycleValue);
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

    public void CreateCard(CardData cardDrawed, Transform parent)
    {
        CardDisplay cardDisplay = Instantiate(cardPrefab, parent).GetComponent<CardDisplay>();
        cardDisplay.SetCard(cardDrawed.Card);
    }

    public void EndTurn()
    {
        StartCoroutine(EndTurnReactiveEffect());
    }

    public IEnumerator EndTurnReactiveEffect()
    {
        yield return player.CheckReactiveEffects(ReactiveTrigger.EndOfTurn);
        player.CheckTurnsReactiveEffects(ReactiveTrigger.EndOfTurn);

        Debug.Log("END PLAYER'S TURN");

        pressedEndTurnButton = true;

        // For enemy state
        enemyTurnQueue.Clear();
        enemyTurnQueue = Extensions.CloneList(enemyList);
        enemyTurnDone = false;
    }

    public void DestroyEnemy(Enemy enemy)
    {
        Destroy(enemy.gameObject);
    }


    /// <summary>
    /// Not using atm
    /// </summary>
    public void ClearCombatScene()
    {
        // Destroy card in hand
        foreach (Transform child in playerHandTransform)
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

    public void EnemyDied()
    {
        ResetSelectedEnemyUI();
        selectedEnemyToAttack.DisableSelection = true;
        selectedEnemyToAttack.SelectionRing.SetActive(false);

        // Remove enemy
        enemyList.Remove(selectedEnemyToAttack as Enemy);
        enemyTurnQueue.Remove(selectedEnemyToAttack as Enemy);
        //ctx.DestroyEnemy(ctx.selectedEnemyToAttack);

        // Are there enemies still alive
        if (enemyList.Count > 0)
        {
            // Select different enemy
            selectedEnemyToAttack = enemyList[0];
            selectedEnemyToAttack.EnemySelection(true);
        }
    }


    public void SpawnDamagePopupUI(Avatar avatar, float damage, Color color)
    {
        CombatUIManager UIManager = combatUIManager;

        GameObject popupObj = Instantiate(UIManager.DamagePopupPrefab, UIManager.WorldSpaceCanvas);
        popupObj.transform.position = new Vector3(avatar.transform.position.x + Random.Range(-UIManager.RandomOffsetHorizontal, UIManager.RandomOffsetHorizontal),
                                                  avatar.transform.position.y + UIManager.OffsetVertical,
                                                  avatar.transform.position.z + Random.Range(-UIManager.RandomOffsetHorizontal, UIManager.RandomOffsetHorizontal));
        Vector3 moveToPos = popupObj.transform.position;
        moveToPos.y += 1f;

        TextMeshProUGUI popupText = popupObj.GetComponent<TextMeshProUGUI>();
        popupText.text = damage.ToString();
        popupText.color = color;

        popupObj.transform.DOMoveY(popupObj.transform.position.y + UIManager.MoveVertical, UIManager.MoveDuration).SetEase(Ease.OutQuad).OnComplete(() =>
        {
            popupText.DOFade(0, UIManager.FadeDuration).OnComplete(() => { Destroy(popupObj); });
        });
    }

    #endregion

}
