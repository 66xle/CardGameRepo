using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using MyBox;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnemyUI : MonoBehaviour
{
    [Header("Images")]
    [MustBeAssigned] [SerializeField] Image CharacterImage;
    [MustBeAssigned] [SerializeField] Image BackLayer;
    [MustBeAssigned] [SerializeField] Image FrontDivider;
    [MustBeAssigned] [SerializeField] Image GuardBarImage;

    [Header("Elite Images")]
    [SerializeField] Image EyeMotifHollow;
    [SerializeField] Image FrontLayer2;
    

    [Header("Text UI")]
    [MustBeAssigned] public TMP_Text HealthText;
    [MustBeAssigned] public TMP_Text MaxHealthText;
    [MustBeAssigned] public TMP_Text BlockText;
    [MustBeAssigned] public Slider GuardBar;
    public TMP_Text Name;

    [Header("Elite Animate")]
    public Transform FrontLayer;
    public float MaxScale = 1.5f;
    public float ScaleDuration = 1f;
    private bool isSelected = false;

    [Header("Minion Animate")]
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

        CharacterImage.sprite = enemy.EnemyData.CharacterSprite;

        rectTransform = GetComponent<RectTransform>();

        ResetColor();
    }

    public void SetUIActive(bool toggle)
    {
        if (!IsMinion)
        {
            if (toggle && isSelected) return;

            if (toggle)
            {
                FrontLayer.DOScale(MaxScale, ScaleDuration / 2).OnComplete(() => FrontLayer.DOScale(1f, ScaleDuration / 2));
                isSelected = true;
            }
            else
            {
                isSelected = false;
            }

            return;
        }


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

    public void ResetColor()
    {
        CharacterImage.color = Color.white;
        BackLayer.color = Color.white;
        FrontDivider.color = Color.white;
        GuardBarImage.color = Color.white;

        if (IsMinion) return;

        // Elite
        FrontLayer.GetComponent<Image>().color = Color.white;
        EyeMotifHollow.color = Color.white;
        FrontLayer2.color = Color.white;
    }

    public void GrayoutUI()
    {
        CharacterImage.color = Color.gray;
        BackLayer.color = Color.gray;
        FrontDivider.color = Color.gray;
        GuardBarImage.color = Color.gray;

        if (IsMinion) return;

        // Elite
        FrontLayer.GetComponent<Image>().color = Color.gray;
        EyeMotifHollow.color = Color.gray;
        FrontLayer2.color = Color.gray;
    }
}
