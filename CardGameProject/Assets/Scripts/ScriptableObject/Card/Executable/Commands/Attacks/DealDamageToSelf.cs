using System.Collections.Generic;
using UnityEngine;

public class DealDamageToSelf : AttackCommand
{
    protected override List<Avatar> GetTargets()
    {
        List<Avatar> targets = new List<Avatar>();

        targets.Add(ExecutableParameters.ctx.player);

        return targets;
    }
}
