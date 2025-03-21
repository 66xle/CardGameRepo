using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;
using Cinemachine;

public class StatusEffectState : CombatBaseState
{
    Avatar currentAvatarSelected;
    bool doRecoverGuardBreak;
    bool isStatusEffectFinished;

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

        currentAvatarSelected.isInStatusActivation = true;
        currentAvatarSelected.doStatusDmg = false;
        doRecoverGuardBreak = false;
        isStatusEffectFinished = false;

        ctx.StartCoroutine(CheckStatusEffect());
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
        if (ctx.player.IsAvatarDead() || ctx.enemyList.Count == 0)
        {
            SwitchState(factory.CombatEnd());
        }

        if (ctx.currentState.ToString() == PLAYERSTATE && !ctx.skipTurn)
        {
            SwitchState(factory.Draw());
        }
        else if (isStatusEffectFinished)
        {
            if (ctx.skipTurn || currentAvatarSelected.IsAvatarDead())
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

    IEnumerator CheckStatusEffect()
    {
        List<StatusEffectData> statusQueue = new List<StatusEffectData>();

        for (int i = currentAvatarSelected.listOfEffects.Count - 1; i >= 0; i--)
        {
            StatusEffectData currentEffect = currentAvatarSelected.listOfEffects[i];

            // Check effect to trigger
            if (currentAvatarSelected.listOfEffects[i].turnRemaining > 0)
            {
                statusQueue.Add(currentEffect);
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
                    doRecoverGuardBreak = true;
                }


                currentAvatarSelected.listOfEffects.RemoveAt(i);

                // Is enemy being selected
                if (currentAvatarSelected == ctx.selectedEnemyToAttack)
                {
                    Enemy enemy = currentAvatarSelected as Enemy;
                    enemy.detailedUI.ResetStatusUI();
                }
            }
        }

        if (doRecoverGuardBreak || statusQueue.Count > 0)
        {
            // Switch camera for enemy
            if (ctx.currentState.ToString() != PLAYERSTATE)
                ActivateStatusCamera();

            if (doRecoverGuardBreak)
            {
                currentAvatarSelected.RecoverGuardBreak();

                yield return new WaitForSeconds(1f);
            }


            foreach (StatusEffectData effect in statusQueue)
            {
                ActivateEffect(effect);

                if (IsAvatarDeadByStatusEffect())
                    yield break;

                yield return new WaitForSeconds(0.5f);
            }

            // Deactive camera
            if (ctx.currentState.ToString() != PLAYERSTATE)
                DeactivateStatusCamera();
        }


        currentAvatarSelected.DisplayStats();

        isStatusEffectFinished = true;
    }

    public void ActivateEffect(StatusEffectData data)
    {
        if (data.effect == Effect.Bleed)
        {
            currentAvatarSelected.ReduceHealth(data.reduceDamagePercentage, data.stacks);
            // Play Bleed effect
        }

        if (data.effect == Effect.Poison)
        {
            currentAvatarSelected.ReduceHealth(data.reduceDamagePercentage);
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

    public bool IsAvatarDeadByStatusEffect()
    {
        if (currentAvatarSelected.IsAvatarDead())
        {
            currentAvatarSelected.GetComponent<Animator>().SetTrigger("Death");

            if (ctx.currentState.ToString() != PLAYERSTATE)
                ctx.EnemyDied();

            return true;
        }

        return false;
    }

    public void ActivateStatusCamera()
    {
        CinemachineVirtualCamera vcam = currentAvatarSelected.transform.parent.GetChild(0).GetComponent<CinemachineVirtualCamera>();
        vcam.Priority = 40;
    }

    public void DeactivateStatusCamera()
    {
        CinemachineVirtualCamera vcam = currentAvatarSelected.transform.parent.GetChild(0).GetComponent<CinemachineVirtualCamera>();
        vcam.Priority = 0;
    }



}
