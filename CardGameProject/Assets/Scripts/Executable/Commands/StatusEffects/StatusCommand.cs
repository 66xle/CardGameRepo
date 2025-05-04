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
        for (int i = 0; i < ExecutableParameters.Targets.Count; i++)
        {
            Avatar avatarToApply = ExecutableParameters.Targets[i];

            if (avatarToApply.IsGameActionInQueue<ApplyStatusEffectGA>())
            {
                // Update damage value
                ApplyStatusEffectGA applyStatusEffectGA = avatarToApply.GetGameActionFromQueue<ApplyStatusEffectGA>() as ApplyStatusEffectGA;
                avatarToApply.queueGameActions.Add(applyStatusEffectGA);
            }
            else
            {
                // Add game action to queue
                ApplyStatusEffectGA applyStatusEffectGA = new(avatarToApply, Effect);
                avatarToApply.queueGameActions.Add(applyStatusEffectGA);

                // ui update here
            }

            ExecutableParameters.Targets[i] = avatarToApply;
        }

        UpdateGameActionQueue();
    }
}
