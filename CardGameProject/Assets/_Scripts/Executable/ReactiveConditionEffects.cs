using MyBox;
using SerializeReferenceEditor;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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

    public ReactiveConditionEffects Clone()
    {
        return new ReactiveConditionEffects
        {
            EffectType = this.EffectType,
            ReactiveTrigger = this.ReactiveTrigger,
            Commands = this.Commands?
                .Select(c => c?.Clone())
                .ToList(),
        };
    }   
}
