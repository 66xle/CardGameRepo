using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class Executable
{
    public virtual bool RequiresMovement => false;
    public abstract IEnumerator Execute(Action<bool> IsConditionTrue);
}
