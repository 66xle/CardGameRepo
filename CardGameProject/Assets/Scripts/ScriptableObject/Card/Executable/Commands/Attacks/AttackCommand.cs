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
            CounterGA counterGA = new(avatarOpponent, avatarPlayingCardController, opponentController);
            ActionSystem.Instance.Perform(counterGA);
        }
        else
        {
            foreach (Avatar avatarToTakeDamage in targets)
            {
                TakeDamageFromWeaponGA takeDamageFromWeaponGA = new(avatarToTakeDamage, ctx, ExecutableParameters.card.value, ExecutableParameters.weapon.type);
                ActionSystem.Instance.Perform(takeDamageFromWeaponGA);
            }
        }

        yield return new WaitWhile(() => !avatarPlayingCard.isAttackFinished);

        // Return to position
        ReturnToPosGA returnToPosGA = new(avatarPlayingCard, ctx);
        ActionSystem.Instance.Perform(returnToPosGA);

        // wait until we return to our spot
        yield return new WaitWhile(() => !returnToPosGA.IsReturnFinished);

        onComplete?.Invoke(true);
    }
}
