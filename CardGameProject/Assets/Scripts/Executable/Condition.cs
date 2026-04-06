using System;
using System.Collections;
using System.Collections.Generic;
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
