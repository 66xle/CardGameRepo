using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusEffectState : CombatBaseState
{
    Avatar currentAvatarSelected;

    public StatusEffectState(CombatStateMachine context, CombatStateFactory combatStateFactory, VariableScriptObject vso) : base(context, combatStateFactory, vso) { }

    public override void EnterState()
    {
        Debug.Log("Status Effect State");

        #region Decide Which Side Acts

        if (ctx.currentSuperState.ToString() == "PlayerState")
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
        
    }

    public override void FixedUpdateState() { }
    public override void ExitState()
    {
        
    }


    public override void CheckSwitchState()
    {
        if (ctx.currentSuperState.ToString() == "PlayerState")
        {
            SwitchState(factory.Draw());
        }
        else
        {
            SwitchState(factory.EnemyDraw());
        }
    }
    public override void InitializeSubState() { }

    void CheckEffect()
    {
        for (int i = currentAvatarSelected.listOfEffects.Count - 1; i >= 0; i--)
        {
            StatusEffectData currentEffect = currentAvatarSelected.listOfEffects[i];

            // Check effect
            ActivateEffect(currentEffect);


            currentAvatarSelected.listOfEffects[i].turnRemaining--;

            // Status Effect expired
            if (currentAvatarSelected.listOfEffects[i].turnRemaining == 0)
            {
                currentAvatarSelected.listOfEffects.RemoveAt(i);
            }
        }

        currentAvatarSelected.DisplayStats();

        CheckSwitchState();
    }

    public void ActivateEffect(StatusEffectData data)
    {
        if (data.effect == Effect.Bleed)
        {
            currentAvatarSelected.ReduceHealth(data.reduceDamagePercentage);
        }
    }

    
}
