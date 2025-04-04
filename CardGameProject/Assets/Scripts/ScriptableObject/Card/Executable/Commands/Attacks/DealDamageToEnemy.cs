using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "DealDamageToEnemy", menuName = "Commands/Attacks/DealDamageToEnemy")]
public class DealDamageToEnemy : AttackCommand
{
    public override bool RequiresMovement => true;

    protected override List<Avatar> GetTargets()
    {
        List<Avatar> targets = new List<Avatar>();

        targets.Add(ExecutableParameters.avatarOpponent);

        return targets;
    }
}
