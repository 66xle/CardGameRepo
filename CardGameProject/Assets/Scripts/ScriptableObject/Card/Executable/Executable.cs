using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Executable : ScriptableObject
{
    public abstract IEnumerator Execute(Action<bool> onComplete);
}
