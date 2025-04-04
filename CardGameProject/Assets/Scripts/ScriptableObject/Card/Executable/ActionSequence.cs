using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro.EditorUtilities;

public class ActionSequence : Command
{
    protected List<Executable> _actionCommands;
    public override bool RequiresMovement => _actionCommands.Exists(cmd => cmd.RequiresMovement);

    public void SetCommands(List<Executable> actionCommands)
    {
        _actionCommands = actionCommands;
    }

    public override IEnumerator Execute(Action<bool> IsConditionTrue)
    {
        CombatStateMachine ctx = ExecutableParameters.ctx;
        Avatar avatarPlayingCard = ExecutableParameters.avatarPlayingCard;
        Avatar avatarOpponent = ExecutableParameters.avatarOpponent;

        bool hasMoved = false;

        if (RequiresMovement && !hasMoved)
        {
            // Trigger move animation | After move GA, reaction will trigger attack GA
            MoveToPosGA moveToPosGA = new(avatarPlayingCard, avatarOpponent, ctx);
            ActionSystem.Instance.Perform(moveToPosGA);

            TriggerAttackAnimGA triggerAttackAnimGA = new(moveToPosGA.avatarPlayingCard, moveToPosGA.ctx);
            moveToPosGA.PostReactions.Add(triggerAttackAnimGA);

            yield return new WaitWhile(() => !avatarPlayingCard.doDamage);

            hasMoved = true;
        }

        foreach (Executable command in _actionCommands)
        {
            bool isConditionTrue = true;
            yield return command.Execute(result => isConditionTrue = result);

            if (!isConditionTrue) break;
        }

        yield return new WaitWhile(() => !avatarPlayingCard.isAttackFinished); // TODO - Rename to isAnimationFinished

        if (hasMoved)
        {
            // Return to position
            ReturnToPosGA returnToPosGA = new(avatarPlayingCard, ctx);
            ActionSystem.Instance.Perform(returnToPosGA);

            // wait until we return to our spot
            yield return new WaitWhile(() => !returnToPosGA.IsReturnFinished);
        }

        yield return new WaitWhile(() => ActionSystem.Instance.IsPerforming);
    }
}

