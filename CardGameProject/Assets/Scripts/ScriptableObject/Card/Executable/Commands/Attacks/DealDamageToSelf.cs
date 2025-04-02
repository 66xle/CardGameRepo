using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DealDamageToSelf", menuName = "Commands/Attacks/DealDamageToSelf")]
public class DealDamageToSelf : AttackCommand
{
    protected override List<Avatar> GetTargets()
    {
        List<Avatar> targets = new List<Avatar>();

        targets.Add(ExecutableParameters.ctx.player);

        return targets;
    }
}
