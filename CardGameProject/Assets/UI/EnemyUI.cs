using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyUI : MonoBehaviour
{
    [SerializeField] GameObject selectedHighlight;
    private bool disableUI;


    private CombatStateMachine stateMachine;
    private Enemy enemy;

    // Start is called before the first frame update
    void Start()
    {
        disableUI = false;
    }

    public void Init(CombatStateMachine stateMachine, Enemy enemy)
    {
        this.stateMachine = stateMachine;
        this.enemy = enemy;
    }

    public void SelectEnemy()
    {
        if (disableUI)
            return;

        stateMachine.ResetSelectedEnemyUI();
        stateMachine.selectedEnemyToAttack = enemy;

        selectedHighlight.SetActive(true);
    }

    public void SetActive(bool toggleUI)
    {
        selectedHighlight.SetActive(toggleUI);
    }

    public void DisableSelection()
    {
        disableUI = true;
    }
}
