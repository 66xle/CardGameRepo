using events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

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

    private async void Attack()
    {
        ctx.displayCard.GetComponent<CardDisplay>().card = ctx.cardPlayed;
        ctx.displayCard.gameObject.SetActive(true);

        await Task.Delay(1000);

        ctx.player.ConsumeStamina(ctx.cardPlayed.cost);

        float damage = ctx.cardPlayed.value;
        ctx.selectedTarget.TakeDamage(damage);

        ctx.displayCard.gameObject.SetActive(false);

        // Attack finished
        ctx.isAttacking = false;
    }

}
