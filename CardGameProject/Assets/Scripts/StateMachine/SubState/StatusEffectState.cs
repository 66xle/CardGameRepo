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
    Avatar _currentAvatarSelected;
    bool _doRecoverGuardBreak;
    bool _isStatusEffectFinished;

    bool _skipTurn;

    public StatusEffectState(CombatStateMachine context, CombatStateFactory combatStateFactory, VariableScriptObject vso) : base(context, combatStateFactory, vso) { }

    public override void EnterState()
    {
        Debug.Log("Status Effect State");

        _skipTurn = false;

        #region Decide Which Side Acts


        if (ctx.currentState.ToString() == PLAYERSTATE)
        {
            _currentAvatarSelected = ctx.player;
        }
        else
        {
            _currentAvatarSelected = ctx.CurrentEnemyTurn;
        }

        #endregion

        _currentAvatarSelected.IsInStatusActivation = true;
        _currentAvatarSelected.DoStatusDmg = false;
        _doRecoverGuardBreak = false;
        _isStatusEffectFinished = false;

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
        if (ctx.player.IsAvatarDead() || ctx.EnemyList.Count == 0)
        {
            SwitchState(factory.CombatEnd());
        }

        if (ctx.currentState.ToString() == PLAYERSTATE && !_skipTurn)
        {
            SwitchState(factory.Draw());
        }
        else if (_isStatusEffectFinished)
        {
            if (_skipTurn || _currentAvatarSelected.IsAvatarDead())
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
        
        // Add effects to active to queue
        for (int i = _currentAvatarSelected.ListOfEffects.Count - 1; i >= 0; i--)
        {
            StatusEffect currentEffect = _currentAvatarSelected.ListOfEffects[i];

            // Check effect to trigger
            if (_currentAvatarSelected.ListOfEffects[i].CurrentTurnsRemaning > 0)
            {
                statusQueue.Add(currentEffect);
            }

            _currentAvatarSelected.ListOfEffects[i].CurrentTurnsRemaning--;

            // Status Effect expired
            if (_currentAvatarSelected.ListOfEffects[i].CurrentTurnsRemaning <= 0)
            {
                // This effect will expire next turn
                if (currentEffect.ShouldRemoveEffectNextTurn())
                {
                    Debug.Log("REMOVE GUARD BREAK NEXT TURN");

                    _currentAvatarSelected.ListOfEffects[i].SetRemoveEffectNextTurn(false);
                    continue;
                }

                if (currentEffect.Effect == Effect.GuardBroken)
                {
                    _doRecoverGuardBreak = true;
                }

                _currentAvatarSelected.ListOfEffects.RemoveAt(i);
                currentEffect.OnRemoval(_currentAvatarSelected);

                // Is enemy being selected
                if (_currentAvatarSelected == ctx._selectedEnemyToAttack)
                {
                    Enemy enemy = _currentAvatarSelected as Enemy;
                    enemy.DetailedUI.ClearStatusUI();
                }
            }
        }


        // Activate Effects in Queue
        if (_doRecoverGuardBreak || statusQueue.Count > 0)
        {
            // Switch camera for enemy
            if (ctx.currentState.ToString() != PLAYERSTATE)
                ActivateStatusCamera();

            if (_doRecoverGuardBreak)
            {
                _currentAvatarSelected.RecoverGuardBreak();

                yield return new WaitForSeconds(1f);
            }


            foreach (StatusEffect effect in statusQueue)
            {
                if (effect.Effect == Effect.GuardBroken)
                {
                    _skipTurn = true;

                    if (ctx.currentState.ToString() == PLAYERSTATE)
                        yield return ctx.EndTurnReactiveEffect();

                    Debug.Log("SKIP TURN");
                }

                if (effect.IsActiveEffect)
                    effect.ActivateEffect(_currentAvatarSelected);

                if (_currentAvatarSelected.IsAvatarDead())
                {
                    _currentAvatarSelected.GetComponent<Animator>().SetTrigger("Death");
                    _currentAvatarSelected.DictReactiveEffects.Clear();

                    if (_currentAvatarSelected is Enemy) ctx.EnemyDied();

                    break;
                }

                yield return new WaitForSeconds(ctx.statusEffectDelay);
            }

            if (statusQueue.Count > 0)
                yield return new WaitForSeconds(ctx.statusEffectAfterDelay);


            // Deactive camera
            if (ctx.currentState.ToString() != PLAYERSTATE)
                DeactivateStatusCamera();
        }


        _currentAvatarSelected.UpdateStatsUI();

        _isStatusEffectFinished = true;


        if (_currentAvatarSelected.IsAvatarDead()) yield return null;

        yield return _currentAvatarSelected.CheckReactiveEffects(ReactiveTrigger.StartOfTurn);
        _currentAvatarSelected.CheckTurnsReactiveEffects(ReactiveTrigger.StartOfTurn);
    }

    public void ActivateStatusCamera()
    {
        CinemachineVirtualCamera vcam = _currentAvatarSelected.transform.parent.GetChild(0).GetComponent<CinemachineVirtualCamera>();
        ctx.CameraManager.ActivateCamera(vcam);
    }

    public void DeactivateStatusCamera()
    {
        ctx.CameraManager.DefaultState();
    }
}
