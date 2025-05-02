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

        avatarPlayingCard.doDamage = false;
        avatarPlayingCard.isAttackFinished = false;
        bool hasMoved = false;

        yield return ReadCommands(_actionCommands);

        if (!avatarOpponent.isRunningReactiveEffect)
            yield return CheckReactiveEffect(avatarPlayingCard, avatarOpponent, ReactiveTrigger.BeforeTakeDamageByWeapon);

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

        if (!avatarOpponent.isRunningReactiveEffect)
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

                CheckReactiveCondition(ExecutableParameters.avatarPlayingCard, currentReactiveCondition);

                
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

    private void CheckReactiveCondition(Avatar avatarPlayingCard, ReactiveCondition currentReactiveCondition)
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



                //if (currentReactiveCondition.GUID != wrapper.ReactiveConditionGUID) continue;

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
            if (!avatarTarget.isTakeDamage) continue;

            if (trigger == ReactiveTrigger.AfterTakeDamageByWeapon) 
                avatarTarget.isTakeDamage = false;

            Debug.Log(trigger);

            ExecutableParameters.avatarPlayingCard = avatarTarget;
            ExecutableParameters.avatarOpponent = avatarPlayingCard;
            List<Avatar> tempQueue = Extensions.CloneList(ExecutableParameters.Queue);
            List<Avatar> tempTargets = Extensions.CloneList(ExecutableParameters.Targets);
            List<GameAction> tempGA = Extensions.CloneList(avatarTarget.queueGameActions);
            avatarTarget.queueGameActions.Clear();

            yield return avatarTarget.CheckReactiveEffects(trigger);

            ExecutableParameters.avatarPlayingCard = avatarPlayingCard;
            ExecutableParameters.avatarOpponent = avatarOpponent;
            ExecutableParameters.Queue = tempQueue;
            ExecutableParameters.Targets = tempTargets;
            avatarTarget.queueGameActions = tempGA;

            avatarPlayingCard.doDamage = false;
            avatarPlayingCard.isAttackFinished = false;
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

