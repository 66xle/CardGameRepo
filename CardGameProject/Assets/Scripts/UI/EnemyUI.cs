using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyUI : MonoBehaviour
{
    [SerializeField] GameObject selectedHighlight;


    private CombatStateMachine stateMachine;
    private Enemy enemy;


    public void Init(CombatStateMachine stateMachine, Enemy enemy)
    {
        this.stateMachine = stateMachine;
        this.enemy = enemy;
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
        stateMachine.ResetSelectedEnemyUI();

        enemy.EnemySelection(true);
    }
}
