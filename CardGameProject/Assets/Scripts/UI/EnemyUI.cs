using System.Collections;
using System.Collections.Generic;
using MyBox;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnemyUI : MonoBehaviour
{
    [SerializeField] GameObject selectedHighlight;
    [MustBeAssigned] public TMP_Text HealthText;
    [MustBeAssigned] public TMP_Text BlockText;
    [MustBeAssigned] public Slider GuardBar;

    private CombatStateMachine stateMachine;
    private Enemy enemy;
    private EnemyManager enemyManager;


    public void Init(CombatStateMachine stateMachine, Enemy enemy, EnemyManager enemyManager)
    {
        this.stateMachine = stateMachine;
        this.enemy = enemy;
        this.enemyManager = enemyManager;
    }

    public void SetUIActive(bool toggle)
    {
        if (selectedHighlight == null) return;

        selectedHighlight.SetActive(toggle);
    }

    /// <summary>
    /// Click on UI
    /// </summary>
    public void SelectEnemy()
    {
        if (enemy.IsAvatarDead()) return;

        stateMachine.ResetSelectedEnemyUI();

        stateMachine._selectedEnemyToAttack = enemy;
        enemyManager.SelectEnemy(enemy);
    }
}
