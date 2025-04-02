using UnityEngine;
using System.Collections.Generic;

public class DealDamageToAllEnemies : AttackCommand
{
    protected override List<Avatar> GetTargets()
    {
        List<Avatar> targets = new List<Avatar>();

        targets.AddRange(ExecutableParameters.ctx.enemyList);

        return targets;
    }
}
