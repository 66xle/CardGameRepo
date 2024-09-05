using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyUI : MonoBehaviour
{
    [SerializeField] GameObject selectedHighlight;
    private bool disableUI;


    private CombatStateMachine stateMachine;
    private Enemy enemy;
    private DetailedUI detailedUI;

    // Start is called before the first frame update
    void Start()
    {
        disableUI = false;
    }

    public void Init(CombatStateMachine stateMachine, Enemy enemy, DetailedUI detailedUI)
    {
        this.stateMachine = stateMachine;
        this.enemy = enemy;
        this.detailedUI = detailedUI;
    }

    public void SelectEnemy()
    {
        if (disableUI)
            return;

        stateMachine.ResetSelectedEnemyUI();
        stateMachine.selectedEnemyToAttack = enemy;

        SetUIActive(true);
    }

    public void SetUIActive(bool toggle)
    {
        selectedHighlight.SetActive(toggle);
        enemy.ToggleArrow(toggle);

        if (toggle)
            ChangeTarget();
    }

    public void DisableSelection()
    {
        disableUI = true;
    }

    public void ChangeTarget()
    {
        detailedUI.ChangeTarget(enemy);
    }
}
