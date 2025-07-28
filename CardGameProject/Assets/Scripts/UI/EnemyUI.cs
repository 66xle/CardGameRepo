using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

public class EnemyUI : MonoBehaviour
{
    [SerializeField] GameObject selectedHighlight;


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
