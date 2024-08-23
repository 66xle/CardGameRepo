using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;

public class StatusEffectState : CombatBaseState
{
    Avatar currentAvatarSelected;

    public StatusEffectState(CombatStateMachine context, CombatStateFactory combatStateFactory, VariableScriptObject vso) : base(context, combatStateFactory, vso) { }

    public override void EnterState()
    {
        Debug.Log("Status Effect State");

        ctx.skipTurn = false;

        #region Decide Which Side Acts




        if (ctx.currentState.ToString() == PLAYERSTATE)
        {
            currentAvatarSelected = ctx.player;
        }
        else
        {
            currentAvatarSelected = ctx.currentEnemyTurn;
        }

        #endregion

        CheckStatusEffect();
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
        if (ctx.currentState.ToString() == PLAYERSTATE && !ctx.skipTurn)
        {
            SwitchState(factory.Draw());
        }
        else
        {
            if (ctx.skipTurn)
            {
                SwitchState(factory.EnemyTurn());
            }
            else
            {
                SwitchState(factory.EnemyDraw());
            }
        }
    }
    public override void InitializeSubState() { }

    void CheckStatusEffect()
    {
        for (int i = currentAvatarSelected.listOfEffects.Count - 1; i >= 0; i--)
        {
            StatusEffectData currentEffect = currentAvatarSelected.listOfEffects[i];

            // Check effect
            if (currentAvatarSelected.listOfEffects[i].turnRemaining > 0)
            {
                ActivateEffect(currentEffect);
                IsAvatarDead();
            }

            currentAvatarSelected.listOfEffects[i].turnRemaining--;

            // Status Effect expired
            if (currentAvatarSelected.listOfEffects[i].turnRemaining <= 0)
            {
                // This effect will expire next turn
                if (currentEffect.removeEffectNextTurn)
                {
                    currentAvatarSelected.listOfEffects[i].removeEffectNextTurn = false;
                    continue;
                }

                if (currentEffect.effect == Effect.GuardBroken)
                {
                    currentAvatarSelected.RecoverGuardBreak();
                }

                

                currentAvatarSelected.listOfEffects.RemoveAt(i);
            }
        }

        currentAvatarSelected.DisplayStats();
    }

    public void ActivateEffect(StatusEffectData data)
    {
        if (data.effect == Effect.Bleed)
        {
            currentAvatarSelected.ReduceHealth(data.reduceDamagePercentage);
            // Play Bleed effect
        }


        if (data.effect == Effect.GuardBroken)
        {
            ctx.skipTurn = true;

            if (ctx.currentState.ToString() == PLAYERSTATE)
                ctx.EndTurn();
            else
                ctx.enemyTurnQueue.Remove(ctx.currentEnemyTurn);

            Debug.Log("SKIP TURN");
        }
    }

    public void IsAvatarDead()
    {
        if (currentAvatarSelected.IsAvatarDead())
        {
            currentAvatarSelected.GetComponent<Animator>().SetTrigger("Death");

            
            ctx.AvatarDeath(currentAvatarSelected);
        }
    }

    
}
