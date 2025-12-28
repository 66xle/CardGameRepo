using SerializeReferenceEditor;
using System.Collections.Generic;
using UnityEngine;

[SRName("Conditions/Has Weakness")]
public class CONDHasWeakness : Condition
{
    public override List<Executable> Commands { get { return commands; } }

    [SerializeReference][SR] public List<Executable> commands;

    public override bool Evaluate()
    {
        List<Avatar> currentTargets = Extensions.CloneList(EXEParameters.Targets);

        for (int i = 0; i < EXEParameters.Targets.Count; i++)
        {
            Avatar avatarOpponent = EXEParameters.Targets[i];

            if (!avatarOpponent.IsGuardReducible(EXEParameters.AvatarPlayingCard.CurrentWeaponData.DamageType))
            {
                currentTargets.Remove(avatarOpponent);
            }
        }

        EXEParameters.Targets = currentTargets;

        if (EXEParameters.Targets.Count > 0)
            return true;

        return false;
    }
}
