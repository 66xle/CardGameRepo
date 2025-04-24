using UnityEngine;
using System.Collections;
using System.Security.Cryptography;
using System.Collections.Generic;
using System;
public abstract class Condition : Executable
{
    public abstract List<Executable> Commands { get; }

    public override IEnumerator Execute(Action<bool> onComplete)
    {
        bool result = Evaluate();
        yield return null;

        onComplete?.Invoke(result);
    }

    public abstract bool Evaluate();
}
