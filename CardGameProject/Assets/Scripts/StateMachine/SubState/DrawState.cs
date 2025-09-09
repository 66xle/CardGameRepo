using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawState : CombatBaseState
{
    public DrawState(CombatStateMachine context, CombatStateFactory combatStateFactory, VariableScriptObject vso) : base(context, combatStateFactory, vso) { }

    public override void EnterState()
    {
        Debug.Log("Draw State");

        ctx.player.RecoverStamina();
        DrawCards();
    }

    public override void UpdateState()
    {
        CheckSwitchState();
    }

    public override void FixedUpdateState() { }
    public override void ExitState() { }
    public override void CheckSwitchState()
    {
        SwitchState(factory.Play());
    }
    public override void InitializeSubState() { }


    void DrawCards()
    {
        CardManager cm = ctx.CardManager;
        cm.DrawCards(cm.CardsToDraw);
    }

    
}
