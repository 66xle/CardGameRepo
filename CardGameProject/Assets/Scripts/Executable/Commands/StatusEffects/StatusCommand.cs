using SerializeReferenceEditor;
using System;
using System.Collections;

[SRHidden]
public class StatusCommand : Command
{
    public virtual StatusEffect Effect => null;

    public override Executable Clone()
    {
        return (StatusCommand)this.MemberwiseClone();
    }

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
