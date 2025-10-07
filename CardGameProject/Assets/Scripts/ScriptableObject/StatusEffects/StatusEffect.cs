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
    Poison,
    Amplify
}

[Serializable]
public abstract class StatusEffect
{
    [ReadOnly] public Effect Effect;
    public string EffectName;
    public int MaxTurnsRemaining = 1;
    public Sprite Sprite;
    [ReadOnly] public int CurrentTurnsRemaning;
    public bool IsActiveEffect = false;
    public bool IsPassiveEffect = false;

    public abstract StatusEffect Clone();

    public virtual void OnApply(Avatar avatar) 
    {
        CurrentTurnsRemaning = MaxTurnsRemaining;
    }

    public virtual void ActivateEffect(Avatar avatar) { }
    public virtual void OnRemoval(Avatar avatar) { }

    public virtual bool ShouldRemoveEffectNextTurn() { return false; }
    public virtual void SetRemoveEffectNextTurn(bool value) { }

    public virtual int GetStacks() { return 0; }

    public virtual float GetDataPopup() { return 0; }
}
