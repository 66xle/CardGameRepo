
using System.Collections;
using System.Collections.Generic;

public class ExecutableWrapper
{
    public Card Card;
    public int Turns;
    public string ReactiveConditionGUID;
    public OverwriteType OverwriteType;
    public StackType StackType;
    public EffectTiming EffectTiming;
    public ReactiveTrigger ReactiveTrigger;
    public DuplicateEffect DuplicateEffect;
    public List<Executable> Commands = new();

    public ExecutableWrapper(Card card, int turns, ReactiveOptions reactiveOptions)
    {
        Card = card;
        Turns = turns;
        EffectTiming = reactiveOptions.EffectTiming;
        ReactiveTrigger = reactiveOptions.ReactiveTrigger;
        DuplicateEffect = reactiveOptions.DuplicateEffect;
        OverwriteType = reactiveOptions.OverwriteType;
        StackType = reactiveOptions.StackType;
    }

    
}
