
using System.Collections;
using System.Collections.Generic;

public class ExecutableWrapper
{
    public CardData CardData;
    public int Turns;
    public string ReactiveConditionGUID;
    public OverwriteType OverwriteType;
    public StackType StackType;
    public EffectTiming EffectTiming;
    public ReactiveTrigger ReactiveTrigger;
    public DuplicateEffect DuplicateEffect;
    public List<Executable> Commands = new();

    public ExecutableWrapper(CardData data, int turns, ReactiveOptions reactiveOptions)
    {
        CardData = data;
        Turns = turns;
        EffectTiming = reactiveOptions.EffectTiming;
        ReactiveTrigger = reactiveOptions.ReactiveTrigger;
        DuplicateEffect = reactiveOptions.DuplicateEffect;
        OverwriteType = reactiveOptions.OverwriteType;
        StackType = reactiveOptions.StackType;
    }

    public ExecutableWrapper(CardData data, List<Executable> commands)
    {
        CardData = data;
        Commands = commands;
    }

    public ExecutableWrapper(CardData data)
    {
        CardData = data;
    }


}
