using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;


public class DealDamage : AttackCommand
{
    public override bool RequiresMovement => true;

    public CardTarget target;

    protected override List<Avatar> GetTargets()
    {
        List<Avatar> targets = new List<Avatar>();

        if (target == CardTarget.Enemy)
        {
            targets.Add(ExecutableParameters.avatarOpponent);
        }
        else if (target  == CardTarget.Enemy)
        {
            targets.AddRange(ExecutableParameters.ctx.enemyList);
        }
        else if (target == CardTarget.Self)
        {
            targets.Add(ExecutableParameters.avatarPlayingCard);
        }

        return targets;
    }

    
}
