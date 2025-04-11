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

[Serializable]
public abstract class Executable
{
    public virtual bool RequiresMovement => false;
    public virtual bool IsReactiveCondition => false;
    public virtual CardTarget CardTarget => CardTarget.PreviousTarget;

    public abstract IEnumerator Execute(Action<bool> IsConditionTrue);
}
