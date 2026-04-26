using System.Collections.Generic;



public abstract class ReactiveCondition : Condition
{
    public abstract ReactiveOptions ReactiveOptions { get; }

    public override abstract List<Executable> Commands { get; }
    public abstract List<ReactiveConditionEffects> Effects { get; }

    public void AddReactiveEffect()
    {
        ReactiveTrigger triggerTemp = ReactiveOptions.ReactiveTrigger;
        if (ReactiveOptions.EffectTiming == EffectTiming.NextTurn) triggerTemp = ReactiveTrigger.StartOfTurn;

        if (!EXEParameters.AvatarPlayingCard.DictReactiveEffects.TryGetValue(triggerTemp, out var list))
        {
            list = new List<EXEWrapper>();
            EXEParameters.AvatarPlayingCard.DictReactiveEffects[triggerTemp] = list;
        }

        int turns = ReactiveOptions.Turns;
        if (ReactiveOptions.EffectDuration == EffectDuration.ThisTurn) turns = 0;
        else if (ReactiveOptions.EffectDuration == EffectDuration.UntilNextTurn) turns = 1;

        EXEWrapper wrapper = new EXEWrapper(EXEParameters.CardRuntime, turns, ReactiveOptions);
        EXEParameters.AvatarPlayingCard.DictReactiveEffects[triggerTemp].Add(wrapper);
    }

    public abstract void SetCommands();

    public virtual void OnApply() { }

}
