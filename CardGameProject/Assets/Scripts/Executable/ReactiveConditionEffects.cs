using System;
using System.Collections.Generic;
using MyBox;
using SerializeReferenceEditor;
using UnityEngine;

public enum EffectType
{   
    Passive,
    Active
}

[SRName("New Effect")]
[Serializable]
public class ReactiveConditionEffects
{
    public EffectType EffectType = EffectType.Passive;
    [ConditionalField(nameof(EffectType), false, EffectType.Active)] public ReactiveTrigger ReactiveTrigger = ReactiveTrigger.StartOfTurn;

    [SerializeReference][SR] public List<Executable> Commands;
}
