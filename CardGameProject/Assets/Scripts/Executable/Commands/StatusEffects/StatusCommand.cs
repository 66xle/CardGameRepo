using System;
using System.Collections;
using UnityEngine;

public class StatusCommand : ActionSequence
{
    public virtual StatusEffect Effect => null;

    public override IEnumerator Execute(Action<bool> IsConditionTrue)
    {


        // Add effect to avatar list
        foreach (Avatar avatar in ExecutableParameters.Targets)
        {
            avatar.ApplyStatusEffect(Effect);
        }

        



        yield return null;
    }
}
