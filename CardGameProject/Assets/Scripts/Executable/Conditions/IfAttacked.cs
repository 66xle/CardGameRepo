using SerializeReferenceEditor;
using UnityEngine;

[SRName("Reactive Conditions/If Attacked")]
public class IfAttacked : Condition
{
    public override bool IsReactiveCondition => true;

    public override bool Evaluate()
    {
        return true;
    }

    public override void AddCommand(Command command)
    {
        ExecutableParameters.avatarPlayingCard.AfterTakeDamageFromWeaponCMD.Add(command);
    }
}
