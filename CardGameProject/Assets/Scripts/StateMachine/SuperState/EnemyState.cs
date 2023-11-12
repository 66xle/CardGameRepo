using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyState : CombatBaseState
{
    public EnemyState(CombatStateMachine context, CombatStateFactory combatStateFactory, VariableScriptObject vso) : base(context, combatStateFactory, vso)
    {
        isRootState = true;
        InitializeSubState();
    }

    public override void EnterState()
    {
        Debug.Log("Enemy State");
    }
    public override void UpdateState()
    {
        CheckSwitchState();
    }

    public override void FixedUpdateState() { }
    public override void ExitState() { }
    public override void InitializeSubState()
    {
        // Attack state
    }

    public override void CheckSwitchState()
    {
        // Switch to player state
    }
}
