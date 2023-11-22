using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState : CombatBaseState
{
    public PlayerState(CombatStateMachine context, CombatStateFactory combatStateFactory, VariableScriptObject vso) : base(context, combatStateFactory, vso)
    {
        isRootState = true;
        InitializeSubState();
    }

    public override void EnterState()
    {
        Debug.Log("Player State");
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
        SetSubState(factory.Draw());
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
}
