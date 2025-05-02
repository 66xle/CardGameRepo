using SerializeReferenceEditor;
using System;
using System.Collections.Generic;
using UnityEngine;



public abstract class ReactiveCondition : Condition
{
    public string GUID { get; } = Guid.NewGuid().ToString();

    public abstract ReactiveOptions ReactiveOptions { get; }

    public override abstract List<Executable> Commands { get; }

    public void AddReactiveEffect()
    {
        if (ReactiveOptions.DuplicateEffect == DuplicateEffect.Stack && ReactiveOptions.StackType == StackType.None) Debug.LogError("Duplicate Effect is set to Stack, but is set to None. Must have a stack type!");
        if (ReactiveOptions.DuplicateEffect == DuplicateEffect.Overwrite && ReactiveOptions.OverwriteType == OverwriteType.None) Debug.LogError("Duplicate Effect is set to Stack, but is set to None. Must have a stack type!");

        ReactiveTrigger triggerTemp = ReactiveOptions.ReactiveTrigger;
        if (ReactiveOptions.EffectTiming == EffectTiming.NextTurn) triggerTemp = ReactiveTrigger.StartOfTurn;

        if (!ExecutableParameters.avatarPlayingCard.DictReactiveEffects.TryGetValue(triggerTemp, out var list))
        {
            list = new List<ExecutableWrapper>();
            ExecutableParameters.avatarPlayingCard.DictReactiveEffects[triggerTemp] = list;
        }

        int turns = ReactiveOptions.Turns;
        if (ReactiveOptions.EffectDuration == EffectDuration.ThisTurn) turns = 0;
        else if (ReactiveOptions.EffectDuration == EffectDuration.UntilNextTurn) turns = 1;

        ExecutableWrapper wrapper = new ExecutableWrapper(ExecutableParameters.card, turns, ReactiveOptions);
        ExecutableParameters.avatarPlayingCard.DictReactiveEffects[triggerTemp].Add(wrapper);
    }

    public abstract void SetCommands();

}
