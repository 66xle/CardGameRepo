using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;
using Cinemachine;
using MyBox;
using Unity.Android.Gradle.Manifest;

public class StatusEffectState : CombatBaseState
{
    Avatar currentAvatarSelected;
    bool doRecoverGuardBreak;
    bool isStatusEffectFinished;

    bool skipTurn;

    public StatusEffectState(CombatStateMachine context, CombatStateFactory combatStateFactory, VariableScriptObject vso) : base(context, combatStateFactory, vso) { }

    public override void EnterState()
    {
        Debug.Log("Status Effect State");

        skipTurn = false;

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

        if (ctx.currentState.ToString() == PLAYERSTATE && !skipTurn)
        {
            SwitchState(factory.Draw());
        }
        else if (isStatusEffectFinished)
        {
            if (skipTurn || currentAvatarSelected.IsAvatarDead())
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
        List<StatusEffect> statusQueue = new List<StatusEffect>();

        for (int i = currentAvatarSelected.listOfEffects.Count - 1; i >= 0; i--)
        {
            StatusEffect currentEffect = currentAvatarSelected.listOfEffects[i];

            // Check effect to trigger
            if (currentAvatarSelected.listOfEffects[i].currentTurnsRemaning > 0)
            {
                statusQueue.Add(currentEffect);
            }

            currentAvatarSelected.listOfEffects[i].currentTurnsRemaning--;

            // Status Effect expired
            if (currentAvatarSelected.listOfEffects[i].currentTurnsRemaning <= 0)
            {
                // This effect will expire next turn
                if (currentEffect.ShouldRemoveEffectNextTurn())
                {
                    Debug.Log("REMOVE GUARD BREAK NEXT TURN");

                    currentAvatarSelected.listOfEffects[i].SetRemoveEffectNextTurn(false);
                    continue;
                }

                if (currentEffect.effect == Effect.GuardBroken)
                {
                    doRecoverGuardBreak = true;
                }

                currentAvatarSelected.listOfEffects.RemoveAt(i);
                currentEffect.OnRemoval(currentAvatarSelected);

                // Is enemy being selected
                if (currentAvatarSelected == ctx.selectedEnemyToAttack)
                {
                    Enemy enemy = currentAvatarSelected as Enemy;
                    enemy.detailedUI.ClearStatusUI();
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


            foreach (StatusEffect effect in statusQueue)
            {
                if (effect.effect == Effect.GuardBroken)
                {
                    skipTurn = true;

                    if (ctx.currentState.ToString() == PLAYERSTATE)
                        yield return ctx.EndTurnReactiveEffect();
                    else
                        ctx.enemyTurnQueue.Remove(ctx.currentEnemyTurn);

                    Debug.Log("SKIP TURN");
                }

                if (effect.isActiveEffect)
                    effect.ActivateEffect(currentAvatarSelected);

                if (currentAvatarSelected.IsAvatarDead())
                {
                    currentAvatarSelected.GetComponent<Animator>().SetTrigger("Death");
                    currentAvatarSelected.DictReactiveEffects.Clear();

                    if (currentAvatarSelected is Enemy) ctx.EnemyDied();

                    yield break;
                }

                yield return new WaitForSeconds(ctx.statusEffectDelay);
            }

            if (statusQueue.Count > 0)
                yield return new WaitForSeconds(ctx.statusEffectAfterDelay);


            // Deactive camera
            if (ctx.currentState.ToString() != PLAYERSTATE)
                DeactivateStatusCamera();
        }


        currentAvatarSelected.UpdateStatsUI();

        isStatusEffectFinished = true;


        yield return currentAvatarSelected.CheckReactiveEffects(ReactiveTrigger.StartOfTurn);
        currentAvatarSelected.CheckTurnsReactiveEffects(ReactiveTrigger.StartOfTurn);
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
