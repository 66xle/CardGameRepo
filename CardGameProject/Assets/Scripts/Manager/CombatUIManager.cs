using MyBox;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CombatUIManager : MonoBehaviour
{
    [Foldout("Popup", true)]
    [MustBeAssigned] public GameObject DamagePopupPrefab;
    [MustBeAssigned] public Transform WorldSpaceCanvas;
    [Range(0, 1)] public float RandomOffsetHorizontal = 0.5f;
    public float OffsetVertical = 1f;
    public float baseScale = 1f;

    [Separator("Animate")]
    public float MoveDuration;
    public float FadeDuration;
    public float MoveVertical;

    [Foldout("Button", true)]
    [MustBeAssigned] public Button EndTurnButton;
    [MustBeAssigned] public Button SwitchButton;

    [Foldout("Turns", true)]
    public float TurnDuration = 1f;
    public float TurnFadeDuration = 1f;
    [MustBeAssigned] public GameObject PlayerTurnUI;
    [MustBeAssigned] public GameObject EnemyTurnUI;

    [Foldout("Player UI", true)]
    [MustBeAssigned] public Slider HealthBar;
    [MustBeAssigned] public TMP_Text HealthValue;
    [MustBeAssigned] public Slider StaminaBar;
    [MustBeAssigned] public TMP_Text StaminaValue;
    [MustBeAssigned] public Slider GuardBar;
    [MustBeAssigned] public TMP_Text GuardValue;
    [MustBeAssigned] public TMP_Text BlockValue;
    [MustBeAssigned] public GameObject PlayerUI;

    [Foldout("Enemy", true)]
    [MustBeAssigned] public GameObject enemyUIPrefab;
    [MustBeAssigned] public DetailedUI detailedUI;

    [Foldout("UI", true)]
    [MustBeAssigned] public GameObject CombatUI;
    [MustBeAssigned] public GameObject SwitchWeaponUI;
    [MustBeAssigned] public GameObject HideUI;
    [MustBeAssigned] public GameObject DetailedUI;

    [Foldout("Managers", true)]
    [MustBeAssigned] public StatsManager StatsManager;


    public void ToggleHideUI(bool toggle)
    {
        HideUI.SetActive(toggle);
        DetailedUI.SetActive(toggle);
        PlayerUI.SetActive(toggle);
    }

    public void InitPlayerUI(Player player)
    {
        player.InitUI(HealthBar, HealthValue, StaminaBar, StaminaValue, BlockValue, GuardBar, GuardValue, StatsManager.ArmourType);
        player.InitStats(StatsManager.CurrentMaxHealth, StatsManager.CurrentMaxStamina, StatsManager.CurrentMaxGuard, StatsManager.Defence, StatsManager.Attack);
    }
}
