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
        ExecutableParameters.Targets = new List<Avatar>();
        ExecutableParameters.Queue = new List<Avatar>();

        avatarPlayingCard.doDamage = false;
        avatarPlayingCard.isAttackFinished = false;
        bool hasMoved = false;

        foreach (Executable command in _actionCommands)
        {
            if (command.IsReactiveCondition)
            {
                currentReactiveCondition = command as Condition;
                currentReactiveCondition.AddReactiveEffect();
                continue;
            }
            else if (currentReactiveCondition != null && command is Executable c)
            {
                currentReactiveCondition.AddExecutable(c);
                continue;
            }

            currentReactiveCondition = null;


            ExecutableParameters.Targets = GetTargets(command.CardTarget);
            ExecutableParameters.CardTarget = command.CardTarget;

            bool isConditionTrue = true;
            yield return command.Execute(result => isConditionTrue = result);

            if (!isConditionTrue) break;
        }

        foreach (Avatar avatarTarget in ExecutableParameters.Queue)
        {
            if (!avatarTarget.isTakeDamage) continue;
            avatarTarget.isTakeDamage = false;

            ExecutableParameters.avatarPlayingCard = avatarOpponent;
            ExecutableParameters.avatarOpponent = avatarPlayingCard;
            List<Avatar> tempQueue = Extensions.CloneList(ExecutableParameters.Queue);
            List<Avatar> tempTargets = Extensions.CloneList(ExecutableParameters.Targets);

            yield return avatarTarget.CheckReactiveEffects(ReactiveTrigger.BeforeTakeDamageByWeapon);


            ExecutableParameters.avatarPlayingCard = avatarPlayingCard;
            ExecutableParameters.avatarOpponent = avatarOpponent;
            ExecutableParameters.Queue = tempQueue;
            ExecutableParameters.Targets = tempTargets;
        }

        if (avatarPlayingCard.IsGuardBroken())
        {
            ExecutableParameters.Queue.ForEach(avatar => avatar.queueGameActions.Clear());
            yield break;
        }
            

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

        foreach (Avatar avatar in ExecutableParameters.Queue)
        {
            // Perform the game actions on themselfs
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

