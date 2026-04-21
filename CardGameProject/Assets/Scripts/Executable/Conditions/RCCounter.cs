using SerializeReferenceEditor;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[SRName("Reactive Conditions/Counter")]
public class RCCounter : ReactiveCondition
{
    public override bool IsReactiveCondition => true;

    public override ReactiveOptions ReactiveOptions { get { return reactiveOptions; } }
    public override List<Executable> Commands { get { return commands; } }
    public override List<ReactiveConditionEffects> Effects { get { return effects; } }

    [SerializeReference][SR] public ReactiveOptions reactiveOptions = new();
    [SerializeReference][SR] public List<Executable> commands;
    [SerializeReference][SR] public List<ReactiveConditionEffects> effects;

    public override Executable Clone()
    {
        return new RCCounter
        {
            reactiveOptions = this.reactiveOptions,
            commands = this.commands?
                .Select(c => c?.Clone())
                .ToList(),
            effects = this.effects?
                .Select(e => e?.Clone())
                .ToList(),
        };
    }

    public override bool Evaluate()
    {
        return true;
    }

    public override void SetCommands()
    {
        ReactiveTrigger triggerTemp = ReactiveOptions.ReactiveTrigger;
        if (ReactiveOptions.EffectTiming == EffectTiming.NextTurn) triggerTemp = ReactiveTrigger.StartOfTurn;

        EXEParameters.AvatarPlayingCard.DictReactiveEffects[triggerTemp][EXEParameters.AvatarPlayingCard.DictReactiveEffects[triggerTemp].Count - 1].Commands = commands;
        EXEParameters.AvatarPlayingCard.DictReactiveEffects[triggerTemp][EXEParameters.AvatarPlayingCard.DictReactiveEffects[triggerTemp].Count - 1].Effects = effects;
    }

    public override void OnApply()
    {
        EXEParameters.AvatarPlayingCard.IsInCounterState = true;

        Animator animator = EXEParameters.AvatarPlayingCard.Animator;
        animator.SetBool("isReady", true);
    }
}
