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
    public override bool RequiresMovement => CheckRequireMovement();

    private List<Executable> _actionCommands;

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

        avatarPlayingCard.DoDamage = false;
        avatarPlayingCard.IsAttackFinished = false;
        bool hasMoved = false;

        yield return ReadCommands(_actionCommands);

        if (!avatarOpponent.IsRunningReactiveEffect)
            yield return CheckReactiveEffect(avatarPlayingCard, avatarOpponent, ReactiveTrigger.BeforeTakeDamageByWeapon);

        if (avatarPlayingCard.IsGuardBroken())
        {
            ExecutableParameters.Queue.ForEach(avatar => avatar.QueueGameActions.Clear());
            yield break;
        }
            

        if (RequiresMovement && !hasMoved)
        {
            // Trigger move animation | After move GA, reaction will trigger attack GA
            MoveToPosGA moveToPosGA = new(avatarPlayingCard, avatarOpponent, ctx);
            ActionSystem.Instance.Perform(moveToPosGA);

            TriggerAttackAnimGA triggerAttackAnimGA = new(moveToPosGA.avatarPlayingCard, moveToPosGA.ctx);
            moveToPosGA.PostReactions.Add(triggerAttackAnimGA);

            yield return new WaitWhile(() => !avatarPlayingCard.DoDamage);

            hasMoved = true;
        }
        else
        {
            avatarPlayingCard.IsAttackFinished = true; // temp fix
        }

        foreach (Avatar avatar in ExecutableParameters.Queue)
        {
            // Perform the game actions on themselfs
            ActionSystem.Instance.PerformQueue(avatar.QueueGameActions);
        }

        ctx.combatUIManager.ToggleHideUI(false);

        yield return new WaitWhile(() => !avatarPlayingCard.IsAttackFinished); // TODO - Rename to isAnimationFinished

        if (hasMoved)
        {
            // Return to position
            ReturnToPosGA returnToPosGA = new(avatarPlayingCard, ctx);
            ActionSystem.Instance.Perform(returnToPosGA);

            // wait until we return to our spot
            yield return new WaitWhile(() => !returnToPosGA.IsReturnFinished);
        }

        if (!avatarOpponent.IsRunningReactiveEffect)
            yield return CheckReactiveEffect(avatarPlayingCard, avatarOpponent, ReactiveTrigger.AfterTakeDamageByWeapon);



        yield return new WaitWhile(() => ActionSystem.Instance.IsPerforming);
    }

    private IEnumerator ReadCommands(List<Executable> actionCommands)
    {
        foreach (Executable command in actionCommands)
        {
            if (command.IsReactiveCondition)
            {
                ReactiveCondition currentReactiveCondition = command as ReactiveCondition;

                CheckDuplicateReactiveCondition(ExecutableParameters.avatarPlayingCard, currentReactiveCondition);

                
                continue;
            }


            ExecutableParameters.Targets = GetTargets(command.CardTarget);
            ExecutableParameters.CardTarget = command.CardTarget;

            bool isConditionTrue = false;
            yield return command.Execute(result => isConditionTrue = result);

            if (isConditionTrue)
            {
                Condition condition = command as Condition;
                yield return ReadCommands(condition.Commands);
            }
        }
    }

    private void CheckDuplicateReactiveCondition(Avatar avatarPlayingCard, ReactiveCondition currentReactiveCondition)
    {
        foreach (ReactiveTrigger trigger in avatarPlayingCard.DictReactiveEffects.Keys)
        {
            List<ExecutableWrapper> listWrapper = Extensions.CloneList(avatarPlayingCard.DictReactiveEffects[trigger]);

            foreach (ExecutableWrapper wrapper in listWrapper)
            {
                if (wrapper.DuplicateEffect != DuplicateEffect.Overwrite) continue;

                // check if reactive effect is the same
                if (wrapper.OverwriteType != currentReactiveCondition.ReactiveOptions.OverwriteType) continue;

                // compare if triggers are the same
                if (trigger != wrapper.ReactiveTrigger) continue;

                // remove condition
                avatarPlayingCard.DictReactiveEffects[trigger].Remove(wrapper);

                Debug.Log("overwrite: " + wrapper.OverwriteType);

                break;
            }
        }

        currentReactiveCondition.AddReactiveEffect();
        currentReactiveCondition.SetCommands();
    }

    private IEnumerator CheckReactiveEffect(Avatar avatarPlayingCard, Avatar avatarOpponent, ReactiveTrigger trigger)
    {
        foreach (Avatar avatarTarget in ExecutableParameters.Queue)
        {
            if (!avatarTarget.IsTakeDamage) continue;

            if (trigger == ReactiveTrigger.AfterTakeDamageByWeapon) 
                avatarTarget.IsTakeDamage = false;

            Debug.Log(trigger);

            ExecutableParameters.avatarPlayingCard = avatarTarget;
            ExecutableParameters.avatarOpponent = avatarPlayingCard;
            List<Avatar> tempQueue = Extensions.CloneList(ExecutableParameters.Queue);
            List<Avatar> tempTargets = Extensions.CloneList(ExecutableParameters.Targets);
            List<GameAction> tempGA = Extensions.CloneList(avatarTarget.QueueGameActions);
            avatarTarget.QueueGameActions.Clear();

            yield return avatarTarget.CheckReactiveEffects(trigger);

            ExecutableParameters.avatarPlayingCard = avatarPlayingCard;
            ExecutableParameters.avatarOpponent = avatarOpponent;
            ExecutableParameters.Queue = tempQueue;
            ExecutableParameters.Targets = tempTargets;
            avatarTarget.QueueGameActions = tempGA;

            avatarPlayingCard.DoDamage = false;
            avatarPlayingCard.IsAttackFinished = false;
        }
    }

    private bool CheckRequireMovement()
    {
        foreach (Executable command in _actionCommands)
        {
            if (command.IsReactiveCondition)
                return false;

            if (command.RequiresMovement) 
                return true;
        }

        return false;
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

