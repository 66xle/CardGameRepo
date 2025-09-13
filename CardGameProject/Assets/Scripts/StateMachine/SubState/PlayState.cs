using events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class PlayState : CombatBaseState
{
    public PlayState(CombatStateMachine context, CombatStateFactory combatStateFactory, VariableScriptObject vso) : base(context, combatStateFactory, vso) { }

    public override void EnterState()
    {
        Debug.Log("Play State");

        ctx.CardManager.UpdateCardsInHand(ctx._selectedEnemyToAttack);

        ctx.CombatUIManager.HideGameplayUI(false);

        ctx._isPlayedCard = false;
        ctx._isPlayState = true;
        ctx.CombatUIManager.EndTurnButton.interactable = true;
    }
    public override void UpdateState()
    {
        CheckSwitchState();
    }

    public override void FixedUpdateState() { }
    public override void ExitState()
    {
        ctx._isPlayState = false;
        ctx.CombatUIManager.EndTurnButton.interactable = false;
    }


    public override void CheckSwitchState()
    {
        // Attack state
        if (ctx._isPlayedCard)
        {
            SwitchState(factory.Action());
        }
    }
    public override void InitializeSubState() { }


    

}
