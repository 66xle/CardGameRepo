using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using MyBox;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnemyState : CombatBaseState
{
    public EnemyState(CombatStateMachine context, CombatStateFactory combatStateFactory, VariableScriptObject vso) : base(context, combatStateFactory, vso)
    {
        isRootState = true;

        ctx.currentSuperState = this.ToString(); // temp debugging
    }

    public override void EnterState()
    {
        Debug.Log("ENEMY STATE");

        ctx.StartCoroutine(ShowTurnUI());
    }
    public override void UpdateState()
    {
        CheckSwitchState();
    }

    public override void FixedUpdateState() { }
    public override void ExitState() { }
    public override void InitializeSubState()
    {
        SetSubState(factory.EnemyTurn());
        currentSubState.EnterState();
    }

    public override void CheckSwitchState()
    {
        // Switch to player state
        if (ctx._enemyTurnDone)
        {
            SwitchState(factory.Player());
        }
    }

    IEnumerator ShowTurnUI()
    {
        GameObject turnUI = ctx.CombatUIManager.EnemyTurnUI;
        AudioManager.Instance.PlaySound(AudioType.UIEnemyTurn);

        turnUI.SetActive(true);
        yield return new WaitForSeconds(ctx.CombatUIManager.TurnDuration);

        Image image = turnUI.GetComponent<Image>();
        TextMeshProUGUI text = turnUI.GetComponentInChildren<TextMeshProUGUI>();
        text.DOFade(0, ctx.CombatUIManager.TurnFadeDuration);
        Tween tween = image.DOFade(0, ctx.CombatUIManager.TurnFadeDuration);
        yield return tween.WaitForCompletion();

        turnUI.SetActive(false);
        image.DOFade(1, 0f);
        text.DOFade(1, 0f);

        InitializeSubState();
    }
}
