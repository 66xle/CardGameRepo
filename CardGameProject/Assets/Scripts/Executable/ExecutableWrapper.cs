
using System.Collections.Generic;

public class ExecutableWrapper
{
    public Card Card;
    public int Turns;
    public EffectTiming EffectTiming;
    public ReactiveTrigger ReactiveTrigger;
    public List<Executable> Commands = new();

    public ExecutableWrapper(Card card, int turns, EffectTiming effectTiming, ReactiveTrigger reactiveTrigger)
    {
        Card = card;
        Turns = turns;
        EffectTiming = effectTiming;
        ReactiveTrigger = reactiveTrigger;
    }
}
