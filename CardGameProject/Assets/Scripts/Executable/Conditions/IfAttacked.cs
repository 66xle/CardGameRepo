using MyBox;
using SerializeReferenceEditor;
using System.Collections.Generic;
using UnityEngine;

[SRName("Reactive Conditions/If Attacked")]
public class IfAttacked : Condition
{
    public override bool IsReactiveCondition => true;

    public override ReactiveTrigger ReactiveTrigger => reactiveTrigger;
    public override EffectTiming EffectTiming => effectTiming;
    public override EffectDuration EffectDuration => effectDuration;
    public override int Turns => turns;

    public EffectTiming effectTiming;
    public ReactiveTrigger reactiveTrigger;
    public EffectDuration effectDuration;
    [ConditionalField(nameof(effectDuration), false, EffectDuration.NumberOfTurns)] public int turns;

    public override bool Evaluate()
    {
        return true;
    }

    public override void AddExecutable(Executable command)
    {
        ReactiveTrigger triggerTemp = ReactiveTrigger;
        if (effectTiming == EffectTiming.NextTurn) triggerTemp = ReactiveTrigger.StartOfTurn;

        ExecutableParameters.avatarPlayingCard.DictReactiveEffects[triggerTemp][ExecutableParameters.avatarPlayingCard.DictReactiveEffects.Count - 1].Commands.Add(command);
    }
}
