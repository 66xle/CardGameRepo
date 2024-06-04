using events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

public class ActionState : CombatBaseState
{
    public ActionState(CombatStateMachine context, CombatStateFactory combatStateFactory, VariableScriptObject vso) : base(context, combatStateFactory, vso) { }

    public override void EnterState()
    {
        Debug.Log("Action State");

        // Attack started
        ctx.isInAction = true;

        PlayCard();
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
            if (!ctx.isInAction)
            {
                SwitchState(factory.Play());
            }
        }
        else
        {
            if (!ctx.isInAction)
            {
                SwitchState(factory.EnemyTurn());
            }
        }
    }
    public override void InitializeSubState() { }

    private async void PlayCard()
    {
        ctx.displayCard.GetComponent<CardDisplay>().card = ctx.cardPlayed;
        ctx.displayCard.gameObject.SetActive(true);

        await Task.Delay(1000);

        float damage = ctx.cardPlayed.value;

        if (ctx.currentSuperState.ToString() == "PlayerState")
        {
            // Enemy take dmg
            ctx.selectedEnemy.TakeDamage(damage);

            // Check if enemy is dead
            if (ctx.selectedEnemy.isDead())
            {
                ctx.enemyList.Remove(ctx.selectedEnemy);

                ctx.DestroyEnemy(ctx.selectedEnemy);

                // Enemies still exist
                if (ctx.enemyList.Count > 0)
                {
                    // Select different enemy
                    ctx.selectedEnemy = ctx.enemyList[0].GetComponent<Enemy>();
                    ctx.selectedEnemy.GetComponent<MeshRenderer>().material = ctx.redMat;
                }
                else if (ctx.enemyList.Count == 0) // No enemies
                {
                    ctx.ClearCombatScene();

                    ctx.eventDisplay.FinishCombatEvent();
                }
            }
        }
        else
        {
            ctx.player.TakeDamage(damage);
        }

        ctx.displayCard.gameObject.SetActive(false);

        // Attack finished
        ctx.isInAction = false;
        Debug.Log("Finished Attacking");
    }

}
