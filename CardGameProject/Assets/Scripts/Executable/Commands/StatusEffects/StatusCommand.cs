using SerializeReferenceEditor;
using System;
using System.Collections;
using UnityEngine;

[SRHidden]
public class StatusCommand : Command
{
    public virtual StatusEffect Effect => null;

    public override IEnumerator Execute(Action<bool> IsConditionTrue)
    {
        ExecuteCommand();
        yield return null;
    }

    public override void ExecuteCommand()
    {
        // Add effect to avatar list
        foreach (Avatar avatar in ExecutableParameters.Targets)
        {
            avatar.ApplyStatusEffect(Effect.Clone());
        }
    }


}
