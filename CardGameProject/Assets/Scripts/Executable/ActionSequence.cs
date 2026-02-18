using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using SerializeReferenceEditor;
using Random = UnityEngine.Random;

[SRHidden]
public class ActionSequence : Executable
{
    public override bool RequiresMovement => CheckRequireMovement();

    private bool IsAttackingAllEnemies;

    private List<Executable> _actionCommands;

    private bool ReactiveSkipAnimation;

    public ActionSequence(List<Executable> actionCommands)
    {
        _actionCommands = actionCommands;
    }

    public override IEnumerator Execute(Action<bool> IsConditionTrue)
    {
        #region Initialize

        CombatStateMachine ctx = EXEParameters.Ctx;
        Avatar avatarPlayingCard = EXEParameters.AvatarPlayingCard;
        Avatar avatarOpponent = EXEParameters.AvatarOpponent;
        EXEParameters.Targets = new List<Avatar>();
        EXEParameters.Queue = new List<Avatar>();
        AnimationWrapper animationWrapper = GetAttackAnimation();

        avatarPlayingCard.DoDamage = false;
        avatarPlayingCard.IsAttackFinished = false;
        avatarPlayingCard.IsCountered = false;
        avatarPlayingCard.IsRecoilDone = false;
        bool hasMoved = false;
        IsAttackingAllEnemies = false;
        ReactiveSkipAnimation = false;

        //ctx.CameraManager.SetDummy(avatarPlayingCard.transform);
        ctx.CameraManager.SetVictimDummy(avatarOpponent.transform, avatarPlayingCard.transform);

        #endregion


        yield return ReadCommands(_actionCommands);

        if (!avatarOpponent.IsRunningReactiveEffect)
            yield return TriggerReactiveEffect(avatarPlayingCard, avatarOpponent, ReactiveTrigger.BeforeTakeDamageByWeapon);

        if (avatarPlayingCard.IsGuardBroken())
        {
            EXEParameters.Queue.ForEach(avatar => avatar.QueueGameActions.Clear());
            yield break;
        }

        if (!animationWrapper.SkipAnimation && !ReactiveSkipAnimation)
        {
            ctx.CombatUIManager.HideGameplayUI(true);
            ctx._selectedEnemyToAttack.EnableSelectionRing(false);
        }

        #region Movement

        if (RequiresMovement && !hasMoved)
        {
            Debug.Log("movement");

            if (IsAttackingAllEnemies)
                ctx.CameraManager.SetVictimDummy(avatarOpponent.transform.parent.parent, avatarPlayingCard.transform);

            // Trigger move animation | After move GA, reaction will trigger attack GA
            GAMoveToPos moveToPosGA = new(avatarPlayingCard, avatarOpponent, IsAttackingAllEnemies, animationWrapper.DistanceOffset, animationWrapper.FollowTimeline, animationWrapper.MoveTime);
            ActionSystem.Instance.Perform(moveToPosGA);

            AudioResource audio = DetermineCorrectAttackSound(avatarOpponent, animationWrapper.AudioResource);

            GATriggerAttackAnim triggerAttackAnimGA = new(moveToPosGA.AvatarPlayingCard, animationWrapper.AnimationName, animationWrapper.AttackTimeline, audio);
            moveToPosGA.PostReactions.Add(triggerAttackAnimGA);

            yield return new WaitWhile(() => !avatarPlayingCard.DoDamage);

            hasMoved = true;
        }
        else // If avatar is not moving
        {
            if (!animationWrapper.SkipAnimation && !ReactiveSkipAnimation)
            {
                if (animationWrapper.IsAttackAnimation)
                {
                    AudioResource audio = DetermineCorrectAttackSound(avatarOpponent, animationWrapper.AudioResource);

                    GATriggerAttackAnim triggerAttackAnimGA = new(EXEParameters.AvatarPlayingCard, animationWrapper.AnimationName, animationWrapper.AttackTimeline, audio, animationWrapper.IsAttackAnimation);
                    ActionSystem.Instance.Perform(triggerAttackAnimGA);
                }
                else
                {
                    GATriggerAnim triggerAnimGA = new(EXEParameters.AvatarPlayingCard, animationWrapper.AnimationName, animationWrapper.AttackTimeline, animationWrapper.AudioResource);
                    ActionSystem.Instance.Perform(triggerAnimGA);
                    Debug.Log("triggerAnimGA");
                }



                yield return new WaitWhile(() => !avatarPlayingCard.DoDamage); // Rename (Wait for animation to finish)
            }
            else
            {
                avatarPlayingCard.IsAttackFinished = true; // temp fix
            }
        }

        #endregion

        foreach (Avatar avatar in EXEParameters.Queue)
        {
            // Perform the game actions on themselfs
            ActionSystem.Instance.PerformQueue(avatar.QueueGameActions);
        }

        //yield return new WaitForEndOfFrame();

        yield return new WaitWhile(() => !avatarPlayingCard.IsAttackFinished);

        if (!hasMoved) // Hard coded fix
            avatarPlayingCard.IsAttackFinished = false;

        // Wait until opponent has finished recoil animation
        if (avatarPlayingCard.IsCountered) // Hard coded
            yield return new WaitWhile(() => !avatarPlayingCard.IsRecoilDone);

        #region Return

        if (hasMoved)
        {
            // Return to position
            GAReturnToPos returnToPosGA = new(avatarPlayingCard);
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

                TriggerReactiveCondition(EXEParameters.AvatarPlayingCard, currentReactiveCondition);

                ReactiveSkipAnimation = true;

                continue;
            }

            ReactiveSkipAnimation = false;

            EXEParameters.Targets = GetTargets(command.CardTarget);
            EXEParameters.CardTarget = command.CardTarget;

            if (command.CardTarget == CardTarget.AllEnemies) 
                IsAttackingAllEnemies = true; // Move to middle position

            bool isConditionTrue = false;
            yield return command.Execute(result => isConditionTrue = result);

            if (isConditionTrue)
            {
                Condition condition = command as Condition;
                yield return ReadCommands(condition.Commands);
            }
        }
    }

    private void TriggerReactiveCondition(Avatar avatarPlayingCard, ReactiveCondition currentReactiveCondition)
    {
        foreach (ReactiveTrigger trigger in avatarPlayingCard.DictReactiveEffects.Keys)
        {
            List<EXEWrapper> listWrapper = Extensions.CloneList(avatarPlayingCard.DictReactiveEffects[trigger]);

            foreach (EXEWrapper wrapper in listWrapper)
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
        foreach (Avatar avatarTarget in EXEParameters.Queue)
        {
            if (!avatarTarget.IsHit) continue;

            if (trigger == ReactiveTrigger.AfterTakeDamageByWeapon) 
                avatarTarget.IsHit = false;

            Debug.Log(trigger);

            #region Avatar Reference

            EXEParameters.AvatarPlayingCard = avatarTarget;
            EXEParameters.AvatarOpponent = avatarPlayingCard;
            CardData tempData = EXEParameters.CardData;
            List<Avatar> tempQueue = Extensions.CloneList(EXEParameters.Queue);
            List<Avatar> tempTargets = Extensions.CloneList(EXEParameters.Targets);
            List<GameAction> tempGA = Extensions.CloneList(avatarTarget.QueueGameActions);
            avatarTarget.QueueGameActions.Clear();

            #endregion

            yield return avatarTarget.CheckReactiveEffects(trigger);

            #region Reset

            EXEParameters.AvatarPlayingCard = avatarPlayingCard;
            EXEParameters.AvatarOpponent = avatarOpponent;
            EXEParameters.Queue = tempQueue;
            EXEParameters.Targets = tempTargets;
            EXEParameters.CardData = tempData;
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
            targets.Add(EXEParameters.AvatarOpponent);
        }
        else if (target == CardTarget.AllEnemies)
        {
            if (EXEParameters.AvatarPlayingCard is Player)
            {
                targets.AddRange(EXEParameters.Ctx.EnemyList);
            }
            else
            {
                targets.Add(EXEParameters.AvatarOpponent);
            }
        }
        else if (target == CardTarget.Self)
        {
            targets.Add(EXEParameters.AvatarPlayingCard);
        }
        else if (target == CardTarget.AllAllies)
        {
            if (EXEParameters.AvatarPlayingCard is Player)
            {
                targets.Add(EXEParameters.AvatarPlayingCard);
            }
            else
            {
                targets.AddRange(EXEParameters.Ctx.EnemyList);
            }
        }
        else if (target == CardTarget.PreviousTarget)
        {
            return EXEParameters.Targets;
        }

        return targets;
    }

    private AnimationWrapper GetAttackAnimation()
    {
        int index = Random.Range(0, EXEParameters.CardData.AnimationList.Count);

        return EXEParameters.CardData.AnimationList[index];
    }

    private AudioResource DetermineCorrectAttackSound(Avatar avatarOpponent, AudioResource attackAudio)
    {
        if (avatarOpponent.IsInCounterState)
            return AudioManager.Instance.CounteredSound;

        return attackAudio;
    }
}

