
using System.Collections;
using System.Collections.Generic;

public class ExecutableWrapper
{
    public Card Card;
    public int Turns;
    public string ReactiveConditionGUID;
    public EffectTiming EffectTiming;
    public ReactiveTrigger ReactiveTrigger;
    public EffectOption EffectOption;
    public List<Executable> Commands = new();

    private int _maxTurns;

    public ExecutableWrapper(Card card, int turns, EffectTiming effectTiming, ReactiveTrigger reactiveTrigger, EffectOption effectOption, string reactiveConditionGUID)
    {
        Card = card;
        Turns = turns;
        EffectTiming = effectTiming;
        ReactiveTrigger = reactiveTrigger;
        EffectOption = effectOption;
        ReactiveConditionGUID = reactiveConditionGUID;

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
