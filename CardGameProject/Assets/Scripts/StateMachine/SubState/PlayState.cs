using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayState : CombatBaseState
{
    public PlayState(CombatStateMachine context, CombatStateFactory combatStateFactory, VariableScriptObject vso) : base(context, combatStateFactory, vso) { }

    public override void EnterState()
    {
        Debug.Log("Play State");
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
    }
    public override void InitializeSubState() { }

}
