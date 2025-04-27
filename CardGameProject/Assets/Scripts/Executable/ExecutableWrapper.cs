
using System.Collections;
using System.Collections.Generic;

public class ExecutableWrapper
{
    public Card Card;
    public int Turns;
    public EffectTiming EffectTiming;
    public ReactiveTrigger ReactiveTrigger;
    public List<Executable> Commands = new();

    private int _maxTurns;

    public ExecutableWrapper(Card card, int turns, EffectTiming effectTiming, ReactiveTrigger reactiveTrigger)
    {
        Card = card;
        Turns = turns;
        EffectTiming = effectTiming;
        ReactiveTrigger = reactiveTrigger;

        _maxTurns = turns;
    }

    public IEnumerator ExecuteCommands()
    {
        ActionSequence actionSequence = new(Commands);
        yield return actionSequence.Execute(null);
    }

    public void Overwrite()
    {
        Turns = _maxTurns;
    }
}
