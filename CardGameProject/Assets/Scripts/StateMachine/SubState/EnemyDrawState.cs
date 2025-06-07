using events;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Playables;

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
        ctx.EnemyTurnQueue.Remove(ctx.CurrentEnemyTurn);
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
            card.Weapon.DamageType = ctx.CurrentEnemyTurn.DamageType;

            ctx.CardManager.EnemyCardQueue.Add(card);
        }
    }
}
