using MyBox;
using SerializeReferenceEditor;
using System.Collections.Generic;
using UnityEngine;

[SRName("Reactive Conditions/If Attacked")]
public class IfAttacked : ReactiveCondition
{
    public override bool IsReactiveCondition => true;

    public override ReactiveOptions ReactiveOptions { get { return reactiveOptions; } }
    public override List<Executable> Commands { get { return commands; } }

    [SerializeReference][SR] public ReactiveOptions reactiveOptions = new();
    [SerializeReference][SR] public List<Executable> commands;

    public override bool Evaluate()
    {
        return true;
    }

    public override void SetCommands()
    {
        ReactiveTrigger triggerTemp = ReactiveOptions.ReactiveTrigger;
        if (ReactiveOptions.EffectTiming == EffectTiming.NextTurn) triggerTemp = ReactiveTrigger.StartOfTurn;

        ExecutableParameters.avatarPlayingCard.DictReactiveEffects[triggerTemp][ExecutableParameters.avatarPlayingCard.DictReactiveEffects.Count - 1].Commands = commands;
    }
}
