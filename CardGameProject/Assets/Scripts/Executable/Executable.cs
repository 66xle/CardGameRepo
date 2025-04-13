using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CardTarget
{
    PreviousTarget,
    Enemy,
    AllEnemies,
    Self
}

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
public abstract class Executable
{
    public virtual bool RequiresMovement => false;
    public virtual bool IsReactiveCondition => false;
    public virtual CardTarget CardTarget => CardTarget.PreviousTarget;

    public abstract IEnumerator Execute(Action<bool> IsConditionTrue);
}
