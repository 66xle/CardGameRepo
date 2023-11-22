using events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayState : CombatBaseState
{
    public PlayState(CombatStateMachine context, CombatStateFactory combatStateFactory, VariableScriptObject vso) : base(context, combatStateFactory, vso) { }

    public override void EnterState()
    {
        Debug.Log("Play State");

        ctx.isPlayedCard = false;
        ctx.cardPlayed = null;
        ctx.selectedTarget = ctx.enemyList[0];
    }
    public override void UpdateState()
    {
        CheckSwitchState();
    }

    public override void FixedUpdateState() { }
    public override void ExitState() { }
    public override void CheckSwitchState()
    {
        // Attack state
        if (ctx.isPlayedCard)
        {
            SwitchState(factory.Attack());
        }
    }
    public override void InitializeSubState() { }


    

}
