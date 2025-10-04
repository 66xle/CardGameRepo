using SerializeReferenceEditor;
using System.Collections.Generic;
using UnityEngine;

[SRName("Conditions/If Weakness")]
public class IfWeakness : Condition
{
    public override List<Executable> Commands { get { return commands; } }

    [SerializeReference][SR] public List<Executable> commands;

    public override bool Evaluate()
    {
        List<Avatar> currentTargets = Extensions.CloneList(ExecutableParameters.Targets);

        for (int i = 0; i < ExecutableParameters.Targets.Count; i++)
        {
            Avatar avatarOpponent = ExecutableParameters.Targets[i];

            if (!avatarOpponent.IsGuardReducible(ExecutableParameters.AvatarPlayingCard.CurrentWeaponData.DamageType))
            {
                currentTargets.Remove(avatarOpponent);
            }
        }

        ExecutableParameters.Targets = currentTargets;

        if (ExecutableParameters.Targets.Count > 0)
            return true;

        return false;
    }
}
