
using System.Collections;
using System.Collections.Generic;

public class ExecutableWrapper
{
    public Card Card;
    public int Turns;
    public string ReactiveConditionGUID;
    public OverwriteType OverwriteType;
    public EffectTiming EffectTiming;
    public ReactiveTrigger ReactiveTrigger;
    public DuplicateEffect DuplicateEffect;
    public List<Executable> Commands = new();

    public ExecutableWrapper(Card card, int turns, EffectTiming effectTiming, ReactiveTrigger reactiveTrigger, DuplicateEffect duplicateEffect, string reactiveConditionGUID, OverwriteType overwriteType)
    {
        Card = card;
        Turns = turns;
        EffectTiming = effectTiming;
        ReactiveTrigger = reactiveTrigger;
        DuplicateEffect = duplicateEffect;
        ReactiveConditionGUID = reactiveConditionGUID;
        OverwriteType = overwriteType;
    }

    public IEnumerator ExecuteCommands()
    {
        ActionSequence actionSequence = new(Commands);
        yield return actionSequence.Execute(null);
    }
}
