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
        if (ctx.currentSuperState.ToString() == "PlayerState")
        {
            if (!ctx.isAttacking)
            {
                SwitchState(factory.Play());
            }
        }
        else
        {
            if (!ctx.isAttacking)
            {
                SwitchState(factory.EnemyTurn());
            }
        }
    }
    public override void InitializeSubState() { }

    private async void Attack()
    {
        ctx.displayCard.GetComponent<CardDisplay>().card = ctx.cardPlayed;
        ctx.displayCard.gameObject.SetActive(true);

        await Task.Delay(1000);

        float damage = ctx.cardPlayed.value;

        if (ctx.currentSuperState.ToString() == "PlayerState")
        {
            ctx.selectedEnemy.TakeDamage(damage);

            // Check if enemy is dead
            if (ctx.selectedEnemy.isDead())
            {
                ctx.enemyList.Remove(ctx.selectedEnemy);

                ctx.DestroyEnemy(ctx.selectedEnemy);

                if (ctx.enemyList.Count > 0)
                {
                    ctx.selectedEnemy = ctx.enemyList[0].GetComponent<Enemy>();
                    ctx.selectedEnemy.GetComponent<MeshRenderer>().material = ctx.redMat;
                }
            }
        }
        else
        {
            ctx.player.TakeDamage(damage);
        }

        ctx.displayCard.gameObject.SetActive(false);

        // Attack finished
        ctx.isAttacking = false;
        Debug.Log("Finished Attacking");
    }

}
