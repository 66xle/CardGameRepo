using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro.EditorUtilities;
using SerializeReferenceEditor;
using DG.Tweening.Core.Easing;


[SRHidden]
public class ActionSequence : Executable
{
    public override bool RequiresMovement => _actionCommands.Exists(cmd => cmd.RequiresMovement);

    private List<Executable> _actionCommands;
    private Condition currentReactiveCondition = null;

    public ActionSequence(List<Executable> actionCommands)
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
            if (command.IsReactiveCondition)
            {
                currentReactiveCondition = command as Condition;
                continue;
            }
            else if (currentReactiveCondition != null && command is Command c) // not conditions
            {
                currentReactiveCondition.AddCommand(c);
                continue;
            }

            currentReactiveCondition = null;


            ExecutableParameters.Targets = GetTargets(command.CardTarget);
            ExecutableParameters.CardTarget = command.CardTarget;

            bool isConditionTrue = true;
            yield return command.Execute(result => isConditionTrue = result);

            if (!isConditionTrue) break;
        }

        foreach (Avatar avatar in ExecutableParameters.Queue)
        {
            ActionSystem.Instance.PerformQueue(avatar.queueGameActions);
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

    private List<Avatar> GetTargets(CardTarget target)
    {
        List<Avatar> targets = new List<Avatar>();

        if (target == CardTarget.Enemy)
        {
            targets.Add(ExecutableParameters.avatarOpponent);
        }
        else if (target == CardTarget.AllEnemies)
        {
            targets.AddRange(ExecutableParameters.ctx.enemyList);
        }
        else if (target == CardTarget.Self)
        {
            targets.Add(ExecutableParameters.avatarPlayingCard);
        }
        else if (target == CardTarget.PreviousTarget)
        {
            return ExecutableParameters.Targets;
        }

        return targets;
    }
}

