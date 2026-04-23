using System.Collections.Generic;

public class EXEWrapper
{
    public CardRuntime CardRuntime;
    public int Turns;
    public string ReactiveConditionGUID;
    public OverwriteType OverwriteType;
    public StackType StackType;
    public EffectTiming EffectTiming;
    public ReactiveTrigger ReactiveTrigger;
    public DuplicateEffect DuplicateEffect;
    public List<Executable> Commands = new();
    public List<ReactiveConditionEffects> Effects = new();

    public EXEWrapper(CardRuntime cardRuntime, int turns, ReactiveOptions reactiveOptions)
    {
        CardRuntime = cardRuntime;
        Turns = turns;
        EffectTiming = reactiveOptions.EffectTiming;
        ReactiveTrigger = reactiveOptions.ReactiveTrigger;
        DuplicateEffect = reactiveOptions.DuplicateEffect;
        OverwriteType = reactiveOptions.OverwriteType;
        StackType = reactiveOptions.StackType;
    }

    public EXEWrapper(CardRuntime cardRuntime, List<Executable> commands, List<ReactiveConditionEffects> effects)
    {
        CardRuntime = cardRuntime;
        Commands = commands;
        Effects = effects;
    }

    public EXEWrapper(CardRuntime cardRuntime)
    {
        CardRuntime = cardRuntime;
    }


}
