using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
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
    public TMP_Text Name;
    public bool IsMinion = false;
    public float SelectedDistance = -30f;
    public float MoveDuration = 1f;

    private CombatStateMachine stateMachine;
    private Enemy enemy;
    private EnemyManager enemyManager;
    private RectTransform rectTransform;

    public void Init(CombatStateMachine stateMachine, Enemy enemy, EnemyManager enemyManager)
    {
        this.stateMachine = stateMachine;
        this.enemy = enemy;
        this.enemyManager = enemyManager;

        rectTransform = GetComponent<RectTransform>();
    }

    public void SetUIActive(bool toggle)
    {
        if (!IsMinion) return;

        Vector2 targetPos = rectTransform.anchoredPosition;

        if (toggle)
        {
            targetPos.x = SelectedDistance;
        }
        else
        {
            targetPos.x = 0f;
        }

        rectTransform.DOAnchorPosX(targetPos.x, MoveDuration, true);
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
