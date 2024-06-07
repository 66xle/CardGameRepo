using events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

public class ActionState : CombatBaseState
{
    Avatar avatarPlayingCard;
    Avatar avatarOpponent;

    public ActionState(CombatStateMachine context, CombatStateFactory combatStateFactory, VariableScriptObject vso) : base(context, combatStateFactory, vso) { }

    public override void EnterState()
    {
        Debug.Log("Action State");

        // Attack started
        ctx.isInAction = true;

        if (ctx.currentSuperState.ToString() == "PlayerState")
        {
            avatarPlayingCard = ctx.player;
            avatarOpponent = ctx.selectedEnemy;

            PlayCard(ctx.cardPlayed);
        }
        else
        {
            avatarPlayingCard = ctx.currentEnemyTurn;
            avatarOpponent = ctx.player;

            // Play all enemy cards in queue
            foreach (Card cardPlayed in ctx.enemyCardQueue)
            {
                PlayCard(cardPlayed);
            }
        }
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

    private async void PlayCard(Card cardPlayed)
    {
        ctx.displayCard.GetComponent<CardDisplay>().card = cardPlayed;
        ctx.displayCard.gameObject.SetActive(true);

        await Task.Delay(1000);

        if (cardPlayed.cardType == Type.Attack)
        {
            float damage = cardPlayed.value;
            avatarOpponent.TakeDamage(damage);

            if (ctx.currentSuperState.ToString() == "PlayerState")
            {
                EnemiesAlive();
            }
        }
        else if (cardPlayed.cardType == Type.Defend)
        {
            float block = cardPlayed.value;

            avatarPlayingCard.AddBlock(block);
        }

        ctx.displayCard.gameObject.SetActive(false);

        // Attack finished
        ctx.isInAction = false;
        Debug.Log("Finished Attacking");
    }


    private void EnemiesAlive()
    {
        // Check if enemy is dead
        if (ctx.selectedEnemy.isDead())
        {
            // Remove and destroy enemy
            ctx.enemyList.Remove(ctx.selectedEnemy);
            ctx.DestroyEnemy(ctx.selectedEnemy);

            // Are there enemies still alive
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
}
