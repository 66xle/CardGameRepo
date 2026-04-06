using System.Collections.Generic;
using UnityEngine;

public class EnemyDrawState : CombatBaseState
{
    public EnemyDrawState(CombatStateMachine context, CombatStateFactory combatStateFactory, VariableScriptObject vso) : base(context, combatStateFactory, vso) { }

    public override void EnterState()
    {
        Debug.Log("Enemy Draw State");

        ctx.CardManager.EnemyCardQueue.Clear();

        DrawCards();
    }
    public override void UpdateState()
    {
        CheckSwitchState();
    }

    public override void FixedUpdateState() { }
    public override void ExitState()
    {

    }

    public override void CheckSwitchState()
    {
        SwitchState(factory.Action());
    }

    public override void InitializeSubState() { }

    public void DrawCards()
    {
        List<CardData> cards = ctx.CurrentEnemyTurn.GetComponent<Enemy>().DrawCards();

        foreach (CardData card in cards)
        {
            ctx.CardManager.EnemyCardQueue.Add(card);
        }
    }
}
