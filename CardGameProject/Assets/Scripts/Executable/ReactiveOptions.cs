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

public enum DuplicateEffect
{
    Stack,
    Overwrite
}

public enum OverwriteType
{
    None,
    Counter,
    Counterattack
}

public enum StackType
{
    None,
    DoDamage,
    Stats
}



[Serializable]
[SRName("Options")]
public class ReactiveOptions
{
    [Tooltip("Determines the behavior when a reaction already exists")] public DuplicateEffect DuplicateEffect = DuplicateEffect.Stack;
    [ConditionalField(nameof(DuplicateEffect), false, DuplicateEffect.Overwrite)] public OverwriteType OverwriteType = OverwriteType.None;
    [ConditionalField(nameof(DuplicateEffect), false, DuplicateEffect.Stack)] public StackType StackType = StackType.None;

    [Header("Behaviour")]
    [Tooltip("Defines when the reaction starts")] public EffectTiming EffectTiming = EffectTiming.Immediate;
    [Tooltip("Defines when the reaction triggers its effect")] public ReactiveTrigger ReactiveTrigger = ReactiveTrigger.StartOfTurn;
    [Tooltip("How long the reaction remains active")] public EffectDuration EffectDuration = EffectDuration.ThisTurn;
    [ConditionalField(nameof(EffectDuration), false, EffectDuration.NumberOfTurns)] public int Turns = 0;
}
