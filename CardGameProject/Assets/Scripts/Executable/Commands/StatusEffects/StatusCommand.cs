using System;
using System.Collections;
using System.Linq;
using SerializeReferenceEditor;
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
        for (int i = 0; i < EXEParameters.Targets.Count; i++)
        {
            Avatar avatarToApply = EXEParameters.Targets[i];

            // Add game action to queue
            GAApplyStatusEffect applyStatusEffectGA = new(avatarToApply, Effect);
            avatarToApply.QueueGameActions.Add(applyStatusEffectGA);

            // ui update here
            

            EXEParameters.Targets[i] = avatarToApply;
        }

        UpdateGameActionQueue();
    }
}
