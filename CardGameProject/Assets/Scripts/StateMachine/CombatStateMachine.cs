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
using UnityEngine.EventSystems;
using PixelCrushers.DialogueSystem;
using UnityEngine.VFX;

public class CombatStateMachine : MonoBehaviour
{
    protected const string PLAYERSTATE = "PlayerState";

    public string currentSuperState; // ONLY FOR DEBUGGING DON'T USE
    [SerializeField] string subState;

    [Foldout("Player", true)]
    [MustBeAssigned] public GameObject PlayerPrefab;
    [ReadOnly] public Transform PlayerSpawnPos;
    [HideInInspector] public Player player;

    [Header("Lists")]
    [HideInInspector] public List<Enemy> EnemyList;
    [HideInInspector] public List<Enemy> EnemyTurnQueue;
    [HideInInspector] public int TurnIndex = 0;
    [HideInInspector] public Enemy CurrentEnemyTurn;

    [Foldout("Camera", true)]
    public float statusEffectDelay = 0.5f;
    public float statusEffectAfterDelay = 1f;

    [Foldout("StatusEffect", true)]
    [MustBeAssigned] public StatusEffectData GuardBreakLightArmourData;
    [MustBeAssigned] public StatusEffectData GuardBreakMediumArmourData;
    [MustBeAssigned] public StatusEffectData GuardBreakHeavyArmourData;

    // DialogueDatabase
    private string ConversationTitle;
    [HideInInspector] public Transform PlayerActor;
    [HideInInspector] public Transform EnemyActor;
    [HideInInspector] public Transform KnightActor;

    [Foldout("References", true)]
    [MustBeAssigned] [SerializeField] StatsManager StatsManager;
    [MustBeAssigned] [SerializeField] SwitchWeaponManager SwitchWeaponManager;
    [MustBeAssigned] [SerializeField] EnemyManager EnemyManager;
    [MustBeAssigned] [SerializeField] CameraSystem CameraSystem;
    [MustBeAssigned] [SerializeField] LevelManager LevelManager; // Editor only
    [MustBeAssigned] public CameraManager CameraManager;
    [MustBeAssigned] public CombatUIManager CombatUIManager;
    [MustBeAssigned] public CardManager CardManager;
    [MustBeAssigned] public RewardManager RewardManager;


    #region Internal Variables

    // Variables
    [Foldout("Private Variables", true)]
    [ReadOnly] public bool _isPlayedCard;
    [ReadOnly] public bool _isPlayState;
    [ReadOnly] public bool _pressedEndTurnButton;
    [ReadOnly] public bool _enemyTurnDone;
    [ReadOnly] public bool _isInPrepState;
    [ReadOnly] public bool _isPlayerLoaded = false;

    [HideInInspector] public CardData _cardPlayed;
    [HideInInspector] public Enemy _selectedEnemyToAttack;

    [HideInInspector] public VariableScriptObject vso; // Not using for now

    private EquipmentHolster _equipmentHolsterScript;


    public CombatBaseState currentState;
    private CombatStateFactory states;
    private DialogueNodeData nodeData;

    #endregion

    private void Awake()
    {
        //SceneInitialize.Instance.Subscribe(Init, 1);
    }

    public void Init()
    {
#if UNITY_EDITOR
        if (!LevelManager.isEnvironmentLoaded) return; // Editor only
#endif

        Debug.Log("combat start");

        ConversationTitle = null;
        _isPlayedCard = false;
        _isPlayState = false;
        _pressedEndTurnButton = false;
        _enemyTurnDone = false;
        _isInPrepState = false;

        EnemyList = new List<Enemy>();
        SpawnPlayer();
        LoadPlayer();
        LoadEnemy();
        CheckForEnemyDialogue();

        CombatUIManager.HideGameplayUI(true);

        states = new CombatStateFactory(this, vso);
        currentState = new PrepState(this, states, vso);
        currentState.EnterState();

        //if (ConversationTitle != null)
        //{
        //    DialogueManager.StartConversation(ConversationTitle, PlayerActor, EnemyActor);
        //    return;
        //}

        InitBattle();
    }

    // Update is called once per frame
    void Update()
    {
#if UNITY_EDITOR
        if (!LevelManager.isEnvironmentLoaded) return; // Editor only
#endif

        if (currentState == null) return;

        currentState.UpdateStates();
        if (currentState.currentSubState != null)
        {
            subState = currentState.currentSubState.ToString();
        }

        if (InputManager.Instance.LeftClickInputDown && _isPlayState && Time.timeScale == 1)
        {
            if (EventSystem.current.IsPointerOverGameObject())
                return;

            if (Input.touchCount > 0 && EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
                return;

            if (_isInPrepState)
                return;

            SelectEnemy();
        }
    }

    public void InitBattle()
    {
        AudioData musicData = GameManager.Instance.CurrentLevelDataLoaded.Music;
        AudioManager.Instance.PlayMusic(musicData);

        _isInPrepState = false;
        CardManager.LoadCards();

        _selectedEnemyToAttack = EnemyList[0];
        EnemyManager.SelectEnemy(_selectedEnemyToAttack);

        if (!GameManager.Instance.IsInTutorial)
            return;

        if (GameManager.Instance.TutorialStage == 1)
        {
            CombatUIManager.InitTutorial();
            CombatUIManager.EndTurnButton.interactable = false;
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
                Enemy enemy = hit.transform.GetComponent<Enemy>();

                if (enemy.IsAvatarDead()) return;

                ResetSelectedEnemyUI();
                
                _selectedEnemyToAttack = enemy;
                EnemyManager.SelectEnemy(enemy);
            }
        }
    }

    public void SpawnPlayer()
    {
        // Spawn Player
        if (_isPlayerLoaded) return;

        player = Instantiate(PlayerPrefab, PlayerSpawnPos).GetComponent<Player>();
        PlayerActor = player.transform;
    }

    private void LoadPlayer()
    {
        if (!_isPlayerLoaded)
        {
            Debug.Log("Load Player");
            _isPlayerLoaded = true;

            CombatUIManager.InitPlayerUI(player);

            // Equipment
            _equipmentHolsterScript = player.GetComponent<EquipmentHolster>();

            if (GameManager.Instance.IsInTutorial)
            {
                SwitchWeaponManager.InitTutorialWeapon();
            }
            else
            {
                SwitchWeaponManager.InitWeaponData();
            }
                
            List<WeaponData> holsterWeapons = SwitchWeaponManager.GetWeaponList();

            if (holsterWeapons.Count > 0)
            {
                _equipmentHolsterScript.SetHolsteredWeapons(holsterWeapons);

                GameObject weaponToEquip = _equipmentHolsterScript.EquippedWeaponObjects.First(weapon => weapon.GetComponent<Weapon>().Guid == SwitchWeaponManager.CurrentMainHand.Guid);
                _equipmentHolsterScript.EquipWeapon(weaponToEquip);
            }
        }

        CameraManager.SetDummy(player.transform);
        CameraManager.DefaultState();
    }

    private void LoadEnemy()
    {
        List<EnemyData> enemyDataList = EnemyManager.GetEnemies();

        EnemyList = EnemyManager.InitEnemies(enemyDataList);

        CombatUIManager.detailedUI.Init(this);
    }

    private void CheckForEnemyDialogue()
    {
        foreach (Enemy enemy in EnemyList)
        {
            if (!enemy.HasDialogue)
                continue;

            ConversationTitle = enemy.EnemyData.ConversationTitle;
            EnemyActor = enemy.transform;

            return;
        }
    }

    public void OnCardPlayed(CardPlayed evt, Card card, string tag)
    {
        if (tag == "Play")
        {
            if (player.hasEnoughStamina(card.Cost))
            {
                if (GameManager.Instance.TutorialStage == 1)
                {
                    GameManager.Instance.TutorialStage = 2;
                }


                player.ConsumeStamina(card.Cost);

                // Get cardData from player hand to move to discard pile
                CardData cardData = CardManager.PlayerHand.First(data => data.Card.InGameGUID == card.InGameGUID);
                CardManager.PlayerHand.Remove(cardData);
                CardManager.DiscardPile.Add(cardData);

                // Destory Card
                CardContainer container = CardManager.PlayerHandTransform.GetComponent<CardContainer>();
                container.DestroyCard(evt.card);

                GameObject mainHandWeapon = _equipmentHolsterScript.RightHand.GetChild(0).gameObject;
                Weapon weaponScript = mainHandWeapon.GetComponent<Weapon>();

                // Swap Weapon
                if (weaponScript.Guid != cardData.Gear.Guid)
                {
                    GameObject holsteredWeapon;

                    foreach (GameObject weaponObj in _equipmentHolsterScript.EquippedWeaponObjects)
                    {
                        Weapon equippedWeapon = weaponObj.GetComponent<Weapon>();

                        if (equippedWeapon.Guid == cardData.Gear.Guid)
                        {
                            holsteredWeapon = weaponObj;
                            WeaponType weaponType = equippedWeapon.WeaponType;

                            player.Animator.SetInteger("WeaponType", 0);
                            if (weaponType == WeaponType.Dagger)
                            {
                                player.Animator.SetInteger("WeaponType", 1);
                            }

                            _equipmentHolsterScript.HolsterWeapon(mainHandWeapon);
                            _equipmentHolsterScript.EquipWeapon(holsteredWeapon);
                        }
                    }
                }


                // Allow to switch to attack state
                _isPlayedCard = true;
                _cardPlayed = cardData;
            }
        }
        else if (tag == "Recycle")
        {
            if (GameManager.Instance.TutorialStage == 1) return;

            // Get cardData from player hand to move to discard pile
            CardData cardData = CardManager.PlayerHand.First(data => data.Card.InGameGUID == card.InGameGUID);
            CardManager.PlayerHand.Remove(cardData);
            CardManager.DiscardPile.Add(cardData);

            // Destory Card
            CardContainer container = CardManager.PlayerHandTransform.GetComponent<CardContainer>();
            container.DestroyCard(evt.card);

            player.RecycleCardToStamina(card.RecycleValue);
        }
    }

    public void ResetSelectedEnemyUI()
    {
        for (int i = 0; i < EnemyList.Count; i++)
        {
            EnemyList[i].EnemySelection(false);
        }
    }

    public void EndGameplay()
    {
        EnemyManager.ClearEnemiesAndUI();
        CardManager.ResetCards();
        
        // Add reward to deck and holster
        if (RewardManager.ListOfRewards.Count > 0)
        {
            GearData gearData = RewardManager.ListOfRewards[0];

            if (gearData is WeaponData)
            {
                SwitchWeaponManager.CreateWeaponData(gearData as WeaponData);
                _equipmentHolsterScript.SetHolsteredWeapons(new List<WeaponData> { gearData as WeaponData });
            }

            CardManager.AddEquipmentCardsToDeck(gearData);
        }
    }

    public void EnableWeaponTrail()
    {
        VisualEffect weaponTrail = _equipmentHolsterScript.RightHand.GetChild(0).GetComponent<VisualEffect>();
        Debug.Log("WEAPON TRAIL");
    }

    public void DisableWeaponTrail()
    {

    }

    #region Used by StateMachine

    public void EndTurn()
    {
        if (GameManager.Instance.TutorialStage == 3)
            GameManager.Instance.TutorialStage = 4;

        StartCoroutine(EndTurnReactiveEffect());
    }

    public IEnumerator EndTurnReactiveEffect()
    {
        yield return player.CheckReactiveEffects(ReactiveTrigger.EndOfTurn);
        player.CheckTurnsReactiveEffects(ReactiveTrigger.EndOfTurn);

        Debug.Log("END PLAYER'S TURN");

        _pressedEndTurnButton = true;

        // For enemy state
        EnemyTurnQueue.Clear();
        EnemyTurnQueue = Extensions.CloneList(EnemyList);
        _enemyTurnDone = false;
    }

    public void DestroyEnemy(Enemy enemy)
    {
        Destroy(enemy.gameObject);
    }

    public void EnemyDied()
    {
        ResetSelectedEnemyUI();
        _selectedEnemyToAttack.DisableSelection = true;
        _selectedEnemyToAttack.SelectionRing.SetActive(false);

        // Remove enemy
        EnemyList.Remove(_selectedEnemyToAttack as Enemy);
        EnemyTurnQueue.Remove(_selectedEnemyToAttack as Enemy);
        //ctx.DestroyEnemy(ctx.selectedEnemyToAttack);

        // Are there enemies still alive
        if (EnemyList.Count > 0)
        {
            // Select different enemy
            _selectedEnemyToAttack = EnemyList[0];
            EnemyManager.SelectEnemy(_selectedEnemyToAttack);
        }
    }

    #endregion

}
