using DG.Tweening;
using events;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DealDamageToEnemy", menuName = "Executable/Commands/DealDamageToEnemy")]
public class DealDamageToEnemy : Command
{
    public override IEnumerator Execute(Action<bool> onComplete)
    {
        CombatStateMachine ctx = ExecutableParameters.ctx;
        Avatar avatarPlayingCard = ExecutableParameters.avatarPlayingCard;
        Avatar avatarOpponent = ExecutableParameters.avatarOpponent;

        Animator avatarPlayingCardController = avatarPlayingCard.GetComponent<Animator>();
        Animator opponentController = avatarOpponent.GetComponent<Animator>();

        // Trigger move animation
        ExecutableParameters.MoveToEnemy?.Invoke();

        yield return new WaitWhile(() => !ExecutableParameters.hasMoved);

        // Trigger attack animation
        avatarPlayingCard.GetComponent<Animator>().SetTrigger("Attack");

        if (avatarPlayingCard.gameObject.CompareTag("Player"))
        {
            ctx.panCam.transform.position = ctx.followCam.transform.position;
            ctx.panCam.transform.rotation = ctx.followCam.transform.rotation;
            ctx.panCam.Priority = 31;
        }

        yield return new WaitWhile(() => !ExecutableParameters.hasAttacked);


        if (avatarOpponent.isInCounterState)
        {
            opponentController.SetBool("isReady", false);
            opponentController.SetTrigger("Counter");

            avatarPlayingCardController.SetTrigger("Recoil");

            avatarOpponent.isInCounterState = false;
        }
        else
        {
            avatarOpponent.TakeDamage(ExecutableParameters.card.value);

            avatarOpponent.UpdateStatsUI();

            ReduceGuard(ExecutableParameters.weapon.type, avatarOpponent);

            if (avatarOpponent.IsAvatarDead())
            {
                avatarOpponent.GetComponent<Animator>().SetTrigger("Death");
            }
            else
            {
                // Apply effect when guard is broken
                if (avatarOpponent.isGuardBroken())
                {
                    // Check if avatar has guard broken effect
                    if (avatarOpponent.hasStatusEffect(Effect.GuardBroken))
                    {
                        //ReduceHitToRecover();
                    }
                    else
                    {
                        ApplyGuardBroken(ctx, avatarOpponent);
                    }
                }
            }
            

            ctx.SpawnDamagePopupUI(avatarOpponent, ExecutableParameters.card.value, Color.white);
        }

        yield return new WaitWhile(() => !avatarPlayingCard.isAttackFinished);

        ExecutableParameters.ReturnToPosition?.Invoke();

        if (avatarPlayingCard.gameObject.CompareTag("Player"))
            ctx.panCam.Priority = 0;

        onComplete?.Invoke(true);
    }

    private void ReduceGuard(DamageType type, Avatar avatarOpponent)
    {
        if (avatarOpponent.armourType == ArmourType.Light && type == DamageType.Slash ||
            avatarOpponent.armourType == ArmourType.Medium && type == DamageType.Pierce ||
            avatarOpponent.armourType == ArmourType.Heavy && type == DamageType.Blunt ||
            avatarOpponent.armourType == ArmourType.None)
        {
            avatarOpponent.ReduceGuard();
        }
    }

    private void ApplyGuardBroken(CombatStateMachine ctx, Avatar avatarOpponent)
    {
        if (avatarOpponent.armourType == ArmourType.Light || avatarOpponent.armourType == ArmourType.None) avatarOpponent.ApplyGuardBreak(ctx.guardBreakLightArmour);
        else if (avatarOpponent.armourType == ArmourType.Medium) avatarOpponent.ApplyGuardBreak(ctx.guardBreakMediumArmour);
        else if (avatarOpponent.armourType == ArmourType.Heavy) avatarOpponent.ApplyGuardBreak(ctx.guardBreakHeavyArmour);
    }

    
}
