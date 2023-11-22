using events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class EnemyDrawState : CombatBaseState
{
    public EnemyDrawState(CombatStateMachine context, CombatStateFactory combatStateFactory, VariableScriptObject vso) : base(context, combatStateFactory, vso) { }

    public override void EnterState()
    {
        Debug.Log("Enemy Draw State");

        ctx.enemyCardQueue.Clear();

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
        SwitchState(factory.EnemyTurn());
    }
    public override void InitializeSubState() { }

    public void DrawCards()
    {
        foreach (Enemy enemy in ctx.enemyList)
        {
            List<Card> cards = enemy.DrawCards();

            ctx.enemyCardQueue.AddRange(cards);
        }
    }


}
