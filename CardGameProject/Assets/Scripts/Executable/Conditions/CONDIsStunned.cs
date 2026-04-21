using SerializeReferenceEditor;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[SRName("Conditions/Is Stunned")]
public class CONDIsStunned : Condition
{
    public override List<Executable> Commands { get { return commands; } }

    [SerializeReference][SR] public List<Executable> commands;

    public override Executable Clone()
    {
        var copy = new CONDIsStunned();
        copy.commands = commands?
            .Select(c => c?.Clone())
            .ToList();
        return copy;
    }

    public override bool Evaluate()
    {
        List<Avatar> currentTargets = Extensions.CloneList(EXEParameters.Targets);

        for (int i = 0; i < EXEParameters.Targets.Count; i++)
        {
            Avatar avatar = EXEParameters.Targets[i];

            if (!avatar.IsGuardBroken())
            {
                currentTargets.Remove(avatar);
            }
        }

        EXEParameters.Targets = currentTargets;

        if (EXEParameters.Targets.Count > 0)
            return true;

        return false;
    }
}
