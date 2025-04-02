using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AttackCommand : TargetCommand
{
    protected abstract override List<Avatar> GetTargets();

    public override IEnumerator Execute(Action<bool> onComplete)
    {
        List<Avatar> targets = GetTargets();

        CombatStateMachine ctx = ExecutableParameters.ctx;
        Avatar avatarPlayingCard = ExecutableParameters.avatarPlayingCard;
        Avatar avatarOpponent = ExecutableParameters.avatarOpponent;

        Animator avatarPlayingCardController = avatarPlayingCard.GetComponent<Animator>();
        Animator opponentController = avatarOpponent.GetComponent<Animator>();

        // Trigger move animation
        MoveToPosGA moveToPosGA = new(avatarPlayingCard, avatarOpponent, ctx);
        ActionSystem.Instance.Perform(moveToPosGA);

        yield return new WaitWhile(() => !moveToPosGA.IsMoveFinished);

        // Trigger attack animation
        TriggerAttackAnimGA triggerAttackAnimGA = new(avatarPlayingCard, ctx);
        ActionSystem.Instance.Perform(triggerAttackAnimGA);

        yield return new WaitWhile(() => !avatarPlayingCard.doDamage);


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
            avatarOpponent.GetComponent<Animator>().SetTrigger("TakeDamage");

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

        // Return to position
        ReturnToPosGA returnToPosGA = new(avatarPlayingCard, ctx);
        ActionSystem.Instance.Perform(returnToPosGA);

        // wait until we return to our spot
        yield return new WaitWhile(() => !returnToPosGA.IsReturnFinished);

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
