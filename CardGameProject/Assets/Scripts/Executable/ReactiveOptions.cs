using MyBox;
using SerializeReferenceEditor;
using System;
using UnityEngine;

public enum ReactiveTrigger
{
    StartOfTurn,
    DrawCard,
    AfterPlayCard,
    EndOfTurn,
    BeforeTakeDamageByWeapon,
    AfterTakeDamageByWeapon,
}
public enum EffectTiming
{
    Immediate,
    NextTurn
}

public enum EffectDuration
{
    ThisTurn,
    UntilNextTurn,
    NumberOfTurns
}

[Serializable]
[SRName("Options")]
public class ReactiveOptions
{
    public EffectTiming EffectTiming = EffectTiming.Immediate;
    public ReactiveTrigger ReactiveTrigger = ReactiveTrigger.StartOfTurn;
    public EffectDuration EffectDuration = EffectDuration.ThisTurn;
    [ConditionalField(nameof(EffectDuration), false, EffectDuration.NumberOfTurns)] public int Turns = 0;
}
