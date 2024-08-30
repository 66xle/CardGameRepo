using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyUI : MonoBehaviour
{
    public GameObject selectedHighlight;

    [HideInInspector] public CombatStateMachine stateMachine;
    [HideInInspector] public Enemy enemy;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SelectEnemy()
    {
        stateMachine.RemoveSelectedEnemyUI();
        stateMachine.selectedEnemyToAttack = enemy;

        selectedHighlight.SetActive(true);
    }
}
