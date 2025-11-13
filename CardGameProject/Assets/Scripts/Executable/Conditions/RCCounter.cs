using MyBox;
using SerializeReferenceEditor;
using System.Collections.Generic;
using UnityEngine;

[SRName("Reactive Conditions/Counter")]
public class RCCounter : ReactiveCondition
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

        ExecutableParameters.AvatarPlayingCard.DictReactiveEffects[triggerTemp][ExecutableParameters.AvatarPlayingCard.DictReactiveEffects[triggerTemp].Count - 1].Commands = commands;
    }

    public override void OnApply()
    {
        ExecutableParameters.AvatarPlayingCard.IsInCounterState = true;

        Animator animator = ExecutableParameters.AvatarPlayingCard.GetComponent<Animator>();
        animator.SetBool("isReady", true);
    }
}
