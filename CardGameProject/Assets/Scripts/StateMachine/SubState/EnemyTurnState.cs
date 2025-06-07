using events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class EnemyTurnState : CombatBaseState
{

    public EnemyTurnState(CombatStateMachine context, CombatStateFactory combatStateFactory, VariableScriptObject vso) : base(context, combatStateFactory, vso) { }

    public override void EnterState()
    {
        Debug.Log("Enemy Turn State");

        DecideEnemyTurn();
    }
    public override void UpdateState()
    {
        CheckSwitchState();
    }

    public override void FixedUpdateState() { }
    public override void ExitState() { }


    public override void CheckSwitchState()
    {
        if (ctx.EnemyTurnQueue.Count > 0)
        {
            SwitchState(factory.StatusEffect());
        }
        else
        {
            ctx.enemyTurnDone = true;
        }
    }

    public override void InitializeSubState() { }

    void DecideEnemyTurn()
    {
        if (ctx.EnemyTurnQueue.Count > 0)
        {
            ctx.CurrentEnemyTurn = ctx.EnemyTurnQueue[0]; // TO DO: Elites move first
        }
    }
}
