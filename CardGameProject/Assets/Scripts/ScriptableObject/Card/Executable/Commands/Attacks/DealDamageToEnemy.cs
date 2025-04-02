using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "DealDamageToEnemy", menuName = "Commands/Attacks/DealDamageToEnemy")]
public class DealDamageToEnemy : AttackCommand
{
    protected override List<Avatar> GetTargets()
    {
        List<Avatar> targets = new List<Avatar>();

        targets.Add(ExecutableParameters.ctx.selectedEnemyToAttack);

        return targets;
    }
}
