using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public abstract class Command : Executable
{
    public abstract override IEnumerator Execute(Action<bool> IsConditionTrue);
}
