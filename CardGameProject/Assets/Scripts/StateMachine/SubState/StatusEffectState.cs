using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;

public class StatusEffectState : CombatBaseState
{
    Avatar currentAvatarSelected;

    bool statusEffectFinished;

    public StatusEffectState(CombatStateMachine context, CombatStateFactory combatStateFactory, VariableScriptObject vso) : base(context, combatStateFactory, vso) { }

    public override void EnterState()
    {
        Debug.Log("Status Effect State");

        ctx.skipTurn = false;
        statusEffectFinished = false;

        #region Decide Which Side Acts

        if (ctx.currentState.ToString() == "PlayerState")
        {
            currentAvatarSelected = ctx.player;
        }
        else
        {
            currentAvatarSelected = ctx.currentEnemyTurn;
        }

        #endregion


        CheckEffect();
    }
    public override void UpdateState()
    {
        CheckSwitchState();
    }

    public override void FixedUpdateState() { }
    public override void ExitState()
    {
        statusEffectFinished = false;
    }


    public override void CheckSwitchState()
    {
        if (!statusEffectFinished)
            return;

        if (ctx.currentState.ToString() == "PlayerState")
        {
            if (!ctx.skipTurn)
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

    void CheckEffect()
    {
        for (int i = currentAvatarSelected.listOfEffects.Count - 1; i >= 0; i--)
        {
            StatusEffectData currentEffect = currentAvatarSelected.listOfEffects[i];

            // Check effect
            if (currentAvatarSelected.listOfEffects[i].turnRemaining > 0)
                ActivateEffect(currentEffect);


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

                currentAvatarSelected.RecoverGuardBreakCheck(currentEffect);

                currentAvatarSelected.listOfEffects.RemoveAt(i);
            }
        }

        currentAvatarSelected.DisplayStats();

        statusEffectFinished = true;
    }

    public void ActivateEffect(StatusEffectData data)
    {
        if (data.effect == Effect.Bleed)
        {
            currentAvatarSelected.ReduceHealth(data.reduceDamagePercentage);
        }
        
        
        
        if (data.effect == Effect.GuardBroken)
        {
            ctx.skipTurn = true;

            ctx.EndTurn();

            Debug.Log("SKIP TURN");
        }
    }

    
}
