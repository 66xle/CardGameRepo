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
using Unity.Android.Gradle.Manifest;

public class CombatStateMachine : MonoBehaviour
{
    protected const string PLAYERSTATE = "PlayerState";

    public string currentSuperState; // ONLY FOR DEBUGGING DON'T USE
    [SerializeField] string subState;


    [Foldout("Player", true)]
    [MustBeAssigned] [SerializeField] GameObject PlayerPrefab;
    public Transform PlayerSpawnPos;
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
    [MustBeAssigned] public CombatUIManager CombatUIManager;
    [MustBeAssigned] public CardManager CardManager;
    [MustBeAssigned] public RewardManager RewardManager;



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

        if (InputManager.Instance.LeftClickInputDown && isPlayState && Time.timeScale == 1)
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

        CameraSystem.SetPlayer(player.transform);
    }

    private void LoadEnemy()
    {
        //List<EnemyObj> enemyObjList = nodeData.enemies;
        List<EnemyData> enemyDataList = EnemyManager.GetEnemies();

        // Spawn Enemy
        for (int i = 0; i < enemyDataList.Count; i++)
        {
            // Init Obj
            Enemy enemy = Instantiate(enemyDataList[i].Prefab, EnemyManager.EnemySpawnPosList[i]).GetComponent<Enemy>();
            enemy.Init(enemyDataList[i]);
            EnemyList.Add(enemy);

            // Init Stats
            if (i == 0)
                CombatUIManager.detailedUI.Init(this);

            GameObject statsUI = Instantiate(CombatUIManager.enemyUIPrefab, EnemyManager.EnemyUISpawnPosList[i].GetComponent<RectTransform>());
            enemy.InitStats(statsUI, CombatUIManager.detailedUI);

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
                CardData cardData = CardManager.PlayerHand.First(data => data.Card.InGameGUID == card.InGameGUID);
                CardManager.PlayerHand.Remove(cardData);
                CardManager.DiscardPile.Add(cardData);

                // Destory Card
                CardContainer container = CardManager.PlayerHandTransform.GetComponent<CardContainer>();
                container.DestroyCard(evt.card);

                GameObject mainHandWeapon = _equipmentHolsterScript.RightHand.GetChild(0).gameObject;
                Weapon weaponScript = mainHandWeapon.GetComponent<Weapon>();

                // Swap Weapon
                if (weaponScript.Guid != cardData.Weapon.Guid)
                {
                    GameObject holsteredWeapon;

                    foreach (GameObject weaponObj in _equipmentHolsterScript.EquippedWeaponObjects)
                    {
                        Weapon equippedWeapon = weaponObj.GetComponent<Weapon>();

                        if (equippedWeapon.Guid == cardData.Weapon.Guid)
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
                isPlayedCard = true;
                cardPlayed = cardData;
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
        EnemyTurnQueue.Clear();
        EnemyTurnQueue = Extensions.CloneList(EnemyList);
        enemyTurnDone = false;
    }

    public void DestroyEnemy(Enemy enemy)
    {
        Destroy(enemy.gameObject);
    }

    public void EnemyDied()
    {
        ResetSelectedEnemyUI();
        selectedEnemyToAttack.DisableSelection = true;
        selectedEnemyToAttack.SelectionRing.SetActive(false);

        // Remove enemy
        EnemyList.Remove(selectedEnemyToAttack as Enemy);
        EnemyTurnQueue.Remove(selectedEnemyToAttack as Enemy);
        //ctx.DestroyEnemy(ctx.selectedEnemyToAttack);

        // Are there enemies still alive
        if (EnemyList.Count > 0)
        {
            // Select different enemy
            selectedEnemyToAttack = EnemyList[0];
            selectedEnemyToAttack.EnemySelection(true);
        }
    }


    public void SpawnDamagePopupUI(Avatar avatar, float damage, Color color)
    {
        CombatUIManager UIManager = CombatUIManager;

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
