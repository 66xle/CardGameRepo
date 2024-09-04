using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyUI : MonoBehaviour
{
    public GameObject selectedHighlight;
    public bool disableUI;


    [HideInInspector] public CombatStateMachine stateMachine;
    [HideInInspector] public Enemy enemy;

    // Start is called before the first frame update
    void Start()
    {
        disableUI = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SelectEnemy()
    {
        if (disableUI)
            return;

        stateMachine.RemoveSelectedEnemyUI();
        stateMachine.selectedEnemyToAttack = enemy;

        selectedHighlight.SetActive(true);
    }
}
