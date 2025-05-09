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

        ctx.cardManager.EnemyCardQueue.Clear();

        DrawCards();
    }
    public override void UpdateState()
    {
        CheckSwitchState();
    }

    public override void FixedUpdateState() { }
    public override void ExitState()
    {
        ctx.enemyTurnQueue.Remove(ctx.currentEnemyTurn);
    }

    public override void CheckSwitchState()
    {
        SwitchState(factory.Action());
    }

    public override void InitializeSubState() { }

    public void DrawCards()
    {
        List<Card> cards = ctx.currentEnemyTurn.GetComponent<Enemy>().DrawCards();

        WeaponData weapon = new WeaponData();
        weapon.DamageType = ctx.currentEnemyTurn.DamageType;

        ctx.cardManager.EnemyCardQueue.AddRange(cards.Select(card => new CardData(weapon, card)));
    }
}
