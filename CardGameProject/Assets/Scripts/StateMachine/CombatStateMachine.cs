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

public class CombatStateMachine : MonoBehaviour
{
    protected const string PLAYERSTATE = "PlayerState";

    public string currentSuperState; // ONLY FOR DEBUGGING DON'T USE
    [SerializeField] string subState;

    [Foldout("Player", true)]
    [MustBeAssigned] [SerializeField] GameObject PlayerPrefab;
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

    [Foldout("References", true)]
    [MustBeAssigned] [SerializeField] StatsManager StatsManager;
    [MustBeAssigned] [SerializeField] SwitchWeaponManager SwitchWeaponManager;
    [MustBeAssigned] [SerializeField] EnemyManager EnemyManager;
    [MustBeAssigned] [SerializeField] CameraSystem CameraSystem;
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

    [HideInInspector] public CardData _cardPlayed;
    [HideInInspector] public Enemy _selectedEnemyToAttack;

    [HideInInspector] public VariableScriptObject vso; // Not using for now

    private EquipmentHolster _equipmentHolsterScript;


    public CombatBaseState currentState;
    private CombatStateFactory states;
    private DialogueNodeData nodeData;

    #endregion


    public void Start()
    {
        _isPlayedCard = false;
        _isPlayState = false;
        _pressedEndTurnButton = false;
        _enemyTurnDone = false;

        EnemyList = new List<Enemy>();

        LoadPlayer();
        LoadEnemy();
        CardManager.LoadCards();

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

        if (InputManager.Instance.LeftClickInputDown && _isPlayState && Time.timeScale == 1)
        {
            if (EventSystem.current.IsPointerOverGameObject())
                return;

            if (Input.touchCount > 0 && EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
                return;

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
                Enemy enemy = hit.transform.GetComponent<Enemy>();

                if (enemy.IsAvatarDead()) return;

                ResetSelectedEnemyUI();
                
                _selectedEnemyToAttack = enemy;
                EnemyManager.SelectEnemy(enemy);
            }
        }
    }

    private void LoadPlayer()
    {
        // Spawn Player
        player = Instantiate(PlayerPrefab, PlayerSpawnPos).GetComponent<Player>();
        CombatUIManager.InitPlayerUI(player);

        // Equipment
        _equipmentHolsterScript = player.GetComponent<EquipmentHolster>();

        SwitchWeaponManager.InitWeaponData();
        List<WeaponData> holsterWeapons = SwitchWeaponManager.GetWeaponList();

        if (holsterWeapons.Count > 0)
        {
            _equipmentHolsterScript.SetHolsteredWeapons(holsterWeapons);

            GameObject weaponToEquip = _equipmentHolsterScript.EquippedWeaponObjects.First(weapon => weapon.GetComponent<Weapon>().Guid == SwitchWeaponManager.CurrentMainHand.Guid);
            _equipmentHolsterScript.EquipWeapon(weaponToEquip);
        }

        CameraManager.SetDummy(player.transform);
        CameraManager.DefaultState();
    }

    private void LoadEnemy()
    {
        List<EnemyData> enemyDataList = EnemyManager.GetEnemies();

        EnemyList = EnemyManager.InitEnemies(enemyDataList);

        CombatUIManager.detailedUI.Init(this);

        _selectedEnemyToAttack = EnemyList[0];
        EnemyManager.SelectEnemy(_selectedEnemyToAttack);
    }

    public void OnCardPlayed(CardPlayed evt, Card card, string tag)
    {
        if (tag == "Play")
        {
            if (player.hasEnoughStamina(card.Cost))
            {
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


    #region Used by StateMachine
    public void CreateCard(CardData cardDrawed, Transform parent)
    {
        CardDisplay cardDisplay = Instantiate(CardManager.CardPrefab, parent).GetComponent<CardDisplay>();
        cardDisplay.SetCard(cardDrawed, cardDrawed.Card);
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
