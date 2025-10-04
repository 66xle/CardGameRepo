using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using SerializeReferenceEditor;
using Random = UnityEngine.Random;

[SRHidden]
public class ActionSequence : Executable
{
    public override bool RequiresMovement => CheckRequireMovement();

    private bool IsAttackingAllEnemies;

    private List<Executable> _actionCommands;

    private bool SkipAnimation;

    public ActionSequence(List<Executable> actionCommands)
    {
        _actionCommands = actionCommands;
    }

    public override IEnumerator Execute(Action<bool> IsConditionTrue)
    {
        #region Initialize

        CombatStateMachine ctx = ExecutableParameters.Ctx;
        Avatar avatarPlayingCard = ExecutableParameters.AvatarPlayingCard;
        Avatar avatarOpponent = ExecutableParameters.AvatarOpponent;
        ExecutableParameters.Targets = new List<Avatar>();
        ExecutableParameters.Queue = new List<Avatar>();

        avatarPlayingCard.DoDamage = false;
        avatarPlayingCard.IsAttackFinished = false;
        avatarPlayingCard.IsCountered = false;
        avatarPlayingCard.IsRecoilDone = false;
        bool hasMoved = false;
        IsAttackingAllEnemies = false;
        SkipAnimation = false;

        //ctx.CameraManager.SetDummy(avatarPlayingCard.transform);
        ctx.CameraManager.SetVictimDummy(avatarOpponent.transform, avatarPlayingCard.transform);

        #endregion

        yield return ReadCommands(_actionCommands);

        if (!avatarOpponent.IsRunningReactiveEffect)
            yield return TriggerReactiveEffect(avatarPlayingCard, avatarOpponent, ReactiveTrigger.BeforeTakeDamageByWeapon);

        if (avatarPlayingCard.IsGuardBroken())
        {
            ExecutableParameters.Queue.ForEach(avatar => avatar.QueueGameActions.Clear());
            yield break;
        }

        if (!SkipAnimation)
            ctx.CombatUIManager.HideGameplayUI(true);

        #region Movement

        if (RequiresMovement && !hasMoved)
        {
            AnimationWrapper animationWrapper = GetAttackAnimation();

            if (IsAttackingAllEnemies)
                ctx.CameraManager.SetVictimDummy(avatarOpponent.transform.parent.parent, avatarPlayingCard.transform);

            // Trigger move animation | After move GA, reaction will trigger attack GA
            MoveToPosGA moveToPosGA = new(avatarPlayingCard, avatarOpponent, IsAttackingAllEnemies, animationWrapper.DistanceOffset, animationWrapper.FollowTimeline);
            ActionSystem.Instance.Perform(moveToPosGA);

            TriggerAttackAnimGA triggerAttackAnimGA = new(moveToPosGA.AvatarPlayingCard, animationWrapper.AnimationName, animationWrapper.AttackTimeline, animationWrapper.AudioType);
            moveToPosGA.PostReactions.Add(triggerAttackAnimGA);

            yield return new WaitWhile(() => !avatarPlayingCard.DoDamage);

            hasMoved = true;
        }
        else
        {
            if (!SkipAnimation)
            {
                AnimationWrapper animationWrapper = GetAttackAnimation();

                TriggerAttackAnimGA triggerAttackAnimGA = new(ExecutableParameters.AvatarPlayingCard, animationWrapper.AnimationName, animationWrapper.AttackTimeline, animationWrapper.AudioType);
                ActionSystem.Instance.Perform(triggerAttackAnimGA);

                yield return new WaitWhile(() => !avatarPlayingCard.DoDamage);
            }
            else
            {
                avatarPlayingCard.IsAttackFinished = true; // temp fix
            }
        }

        #endregion

        foreach (Avatar avatar in ExecutableParameters.Queue)
        {
            // Perform the game actions on themselfs
            ActionSystem.Instance.PerformQueue(avatar.QueueGameActions);
        }


        yield return new WaitWhile(() => !avatarPlayingCard.IsAttackFinished);

        if (!hasMoved) // Hard coded fix
            avatarPlayingCard.IsAttackFinished = false;

        // Wait until opponent has finished attacking
        if (avatarPlayingCard.IsCountered) // Hard coded (Counter with attack, not counter only)
            yield return new WaitWhile(() => !avatarPlayingCard.IsRecoilDone);

        #region Return

        if (hasMoved)
        {
            // Return to position
            ReturnToPosGA returnToPosGA = new(avatarPlayingCard);
            ActionSystem.Instance.Perform(returnToPosGA);

            // wait until we return to our spot
            yield return new WaitWhile(() => !returnToPosGA.IsReturnFinished);
        }

        #endregion

        if (!avatarOpponent.IsRunningReactiveEffect)
            yield return TriggerReactiveEffect(avatarPlayingCard, avatarOpponent, ReactiveTrigger.AfterTakeDamageByWeapon);


        yield return new WaitWhile(() => ActionSystem.Instance.IsPerforming);
    }

    private IEnumerator ReadCommands(List<Executable> actionCommands)
    {
        foreach (Executable command in actionCommands)
        {
            if (command.IsReactiveCondition)
            {
                ReactiveCondition currentReactiveCondition = command as ReactiveCondition;

                TriggerDuplicateReactiveCondition(ExecutableParameters.AvatarPlayingCard, currentReactiveCondition);

                SkipAnimation = true;

                continue;
            }

            SkipAnimation = false;

            ExecutableParameters.Targets = GetTargets(command.CardTarget);
            ExecutableParameters.CardTarget = command.CardTarget;

            if (command.CardTarget == CardTarget.AllEnemies) 
                IsAttackingAllEnemies = true;

            bool isConditionTrue = false;
            yield return command.Execute(result => isConditionTrue = result);

            if (command is DrawCommand)
                SkipAnimation = true;

            if (isConditionTrue)
            {
                Condition condition = command as Condition;
                yield return ReadCommands(condition.Commands);
            }
        }
    }

    private void TriggerDuplicateReactiveCondition(Avatar avatarPlayingCard, ReactiveCondition currentReactiveCondition)
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
        currentReactiveCondition.OnApply();
    }

    private IEnumerator TriggerReactiveEffect(Avatar avatarPlayingCard, Avatar avatarOpponent, ReactiveTrigger trigger)
    {
        foreach (Avatar avatarTarget in ExecutableParameters.Queue)
        {
            if (!avatarTarget.IsHit) continue;

            if (trigger == ReactiveTrigger.AfterTakeDamageByWeapon) 
                avatarTarget.IsHit = false;

            Debug.Log(trigger);

            #region Avatar Reference

            ExecutableParameters.AvatarPlayingCard = avatarTarget;
            ExecutableParameters.AvatarOpponent = avatarPlayingCard;
            List<Avatar> tempQueue = Extensions.CloneList(ExecutableParameters.Queue);
            List<Avatar> tempTargets = Extensions.CloneList(ExecutableParameters.Targets);
            List<GameAction> tempGA = Extensions.CloneList(avatarTarget.QueueGameActions);
            avatarTarget.QueueGameActions.Clear();

            #endregion

            yield return avatarTarget.CheckReactiveEffects(trigger);

            #region Reset

            ExecutableParameters.AvatarPlayingCard = avatarPlayingCard;
            ExecutableParameters.AvatarOpponent = avatarOpponent;
            ExecutableParameters.Queue = tempQueue;
            ExecutableParameters.Targets = tempTargets;
            avatarTarget.QueueGameActions = tempGA;

            avatarPlayingCard.DoDamage = false;
            avatarPlayingCard.IsAttackFinished = false;

            #endregion
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
            targets.Add(ExecutableParameters.AvatarOpponent);
        }
        else if (target == CardTarget.AllEnemies)
        {
            if (ExecutableParameters.AvatarPlayingCard is Player)
            {
                targets.AddRange(ExecutableParameters.Ctx.EnemyList);
            }
            else
            {
                targets.Add(ExecutableParameters.AvatarOpponent);
            }
        }
        else if (target == CardTarget.Self)
        {
            targets.Add(ExecutableParameters.AvatarPlayingCard);
        }
        else if (target == CardTarget.PreviousTarget)
        {
            return ExecutableParameters.Targets;
        }

        return targets;
    }

    private AnimationWrapper GetAttackAnimation()
    {
        int index = Random.Range(0, ExecutableParameters.CardData.AnimationList.Count);

        return ExecutableParameters.CardData.AnimationList[index];
    }
}

