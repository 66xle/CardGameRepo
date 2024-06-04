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

        CheckQueue();
    }
    public override void UpdateState()
    {
        CheckSwitchState();
    }

    public override void FixedUpdateState() { }
    public override void ExitState() 
    {
        if (ctx.enemyCardQueue.Count > 0)
            ctx.enemyCardQueue.RemoveAt(0);
    }


    public override void CheckSwitchState()
    {
        if (ctx.enemyCardQueue.Count > 0)
        {
            SwitchState(factory.Action());
        }
    }
    public override void InitializeSubState() { }

    void CheckQueue()
    {
        if (ctx.enemyCardQueue.Count > 0)
        {
            ctx.cardPlayed = ctx.enemyCardQueue[0];
            
        }
        else
        {
            ctx.enemyTurnDone = true;
        }
    }
}
