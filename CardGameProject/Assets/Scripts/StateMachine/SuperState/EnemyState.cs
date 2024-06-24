using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyState : CombatBaseState
{
    public EnemyState(CombatStateMachine context, CombatStateFactory combatStateFactory, VariableScriptObject vso) : base(context, combatStateFactory, vso)
    {
        isRootState = true;

        ctx.currentSuperState = this.ToString();
    }

    public override void EnterState()
    {
        Debug.Log("Enemy State");

        InitializeSubState();
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
        if (ctx.enemyTurnDone)
        {
            SwitchState(factory.Player());
        }
    }
}
