using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using UnityEditor;
using UnityEngine;
using MyBox;

public enum Effect
{
    Stunned,
    GuardBroken,
    Bleed,
    Poison
}

[Serializable]
public abstract class StatusEffect
{
    [HideInInspector] public Effect effect;
    public string effectName;
    public int maxTurnsRemaining = 1;
    [ReadOnly] public int currentTurnsRemaning;
    [ReadOnly] public bool isActiveEffect = false;

    public virtual void OnApply(Avatar avatar) 
    {
        currentTurnsRemaning = maxTurnsRemaining;
    }

    public virtual void ActivateEffect(Avatar avatar) { }
    public virtual void OnRemoval(Avatar avatar) { }

    public virtual bool ShouldRemoveEffectNextTurn() { return false; }
    public virtual void SetRemoveEffectNextTurn(bool value) { }

    public virtual int GetStacks() { return 0; }

    
}
