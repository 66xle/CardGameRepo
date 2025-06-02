using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using MyBox;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerState : CombatBaseState
{
    public PlayerState(CombatStateMachine context, CombatStateFactory combatStateFactory, VariableScriptObject vso) : base(context, combatStateFactory, vso)
    {
        isRootState = true;

        ctx.currentSuperState = this.ToString(); // temp debugging
    }

    public override void EnterState()
    {
        Debug.Log("PLAYER STATE");

        ctx.StartCoroutine(ShowTurnUI());
    }
    public override void UpdateState()
    {
        CheckSwitchState();
    }

    public override void FixedUpdateState() { }
    public override void ExitState() 
    {
        ctx.pressedEndTurnButton = false;
    }
    public override void InitializeSubState()
    {
        SetSubState(factory.StatusEffect());
        currentSubState.EnterState();
    }

    public override void CheckSwitchState()
    {
        // Switch to enemy state
        if (ctx.pressedEndTurnButton)
        {
            SwitchState(factory.Enemy());
        }
    }

    IEnumerator ShowTurnUI()
    {
        GameObject turnUI = ctx.combatUIManager.PlayerTurnUI;

        turnUI.SetActive(true);
        yield return new WaitForSeconds(ctx.combatUIManager.TurnDuration);

        Image image = turnUI.GetComponent<Image>();
        TextMeshProUGUI text = turnUI.GetComponentInChildren<TextMeshProUGUI>();
        text.DOFade(0, ctx.combatUIManager.TurnFadeDuration);
        Tween tween = image.DOFade(0, ctx.combatUIManager.TurnFadeDuration);
        yield return tween.WaitForCompletion();

        turnUI.SetActive(false);
        image.DOFade(1, 0f);
        text.DOFade(1, 0f);

        InitializeSubState();
    }
}
