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

        ctx.isPlayedCard = false;
        ctx.cardPlayed = null;
        

        ctx.isPlayState = true;
        ctx.combatUIManager.endTurnButton.interactable = true;
    }
    public override void UpdateState()
    {
        CheckSwitchState();
    }

    public override void FixedUpdateState() { }
    public override void ExitState()
    {
        ctx.isPlayState = false;
        ctx.combatUIManager.endTurnButton.interactable = false;
    }


    public override void CheckSwitchState()
    {
        // Attack state
        if (ctx.isPlayedCard)
        {
            SwitchState(factory.Action());
        }
    }
    public override void InitializeSubState() { }


    

}
