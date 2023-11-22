using events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : CombatBaseState
{

    public AttackState(CombatStateMachine context, CombatStateFactory combatStateFactory, VariableScriptObject vso) : base(context, combatStateFactory, vso) { }

    public override void EnterState()
    {
        Debug.Log("Attack State");

        // Attack started
        ctx.isAttacking = true;

        Attack();
    }
    public override void UpdateState()
    {
        CheckSwitchState();
    }

    public override void FixedUpdateState() { }
    public override void ExitState() { }
    public override void CheckSwitchState()
    {
        if (!ctx.isAttacking)
        {
            SwitchState(factory.Play());
        }
    }
    public override void InitializeSubState() { }

    void Attack()
    {
        ctx.player.ConsumeStamina(ctx.cardPlayed.cost);

        float damage = ctx.cardPlayed.value;
        ctx.selectedTarget.TakeDamage(damage);

        // Attack finished
        ctx.isAttacking = false;
    }

}
