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
        
        if (!GameManager.Instance.IsInTutorial)
            ctx.CombatUIManager.EndTurnButton.interactable = true;

        if (GameManager.Instance.TutorialStage == 2)
        {
            GameManager.Instance.TutorialStage = 3;

            ctx.CombatUIManager.EndTurnButton.interactable = true;
            ctx.CombatUIManager.StartTutorialConversation(1);
        }
        else if (GameManager.Instance.TutorialStage == 4)
        {
            ctx.CombatUIManager.StartTutorialConversation(2);
        }
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
