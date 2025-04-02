using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "DealDamageToAllEnemies", menuName = "Commands/Attacks/DealDamageToAllEnemies")]
public class DealDamageToAllEnemies : AttackCommand
{
    protected override List<Avatar> GetTargets()
    {
        List<Avatar> targets = new List<Avatar>();

        targets.AddRange(ExecutableParameters.ctx.enemyList);

        return targets;
    }
}
