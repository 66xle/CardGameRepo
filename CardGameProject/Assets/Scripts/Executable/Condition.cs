using UnityEngine;
using System.Collections;
using System.Security.Cryptography;
using System.Collections.Generic;
using System;
public abstract class Condition : Executable
{
    public virtual ReactiveTrigger ReactiveTrigger => ReactiveTrigger.StartOfTurn;
    public virtual EffectTiming EffectTiming => EffectTiming.Immediate;
    public virtual EffectDuration EffectDuration => EffectDuration.ThisTurn;
    public virtual int Turns => 0;

    public override IEnumerator Execute(Action<bool> onComplete)
    {
        bool result = Evaluate();
        yield return null;

        onComplete?.Invoke(result);
    }

    public abstract bool Evaluate();

    public virtual void AddReactiveEffect() 
    {
        ReactiveTrigger triggerTemp = ReactiveTrigger;
        if (EffectTiming == EffectTiming.NextTurn) triggerTemp = ReactiveTrigger.StartOfTurn;

        if (!ExecutableParameters.avatarPlayingCard.DictReactiveEffects.TryGetValue(triggerTemp, out var list))
        {
            list = new List<ExecutableWrapper>();
            ExecutableParameters.avatarPlayingCard.DictReactiveEffects[triggerTemp] = list;
        }

        int turns = Turns;
        if (EffectDuration == EffectDuration.ThisTurn) turns = 0;
        else if (EffectDuration == EffectDuration.UntilNextTurn) turns = 1;

        ExecutableWrapper wrapper = new ExecutableWrapper(ExecutableParameters.card, turns, EffectTiming, ReactiveTrigger);
        ExecutableParameters.avatarPlayingCard.DictReactiveEffects[triggerTemp].Add(wrapper);
    }

    public virtual void AddExecutable(Executable command) { }
}
