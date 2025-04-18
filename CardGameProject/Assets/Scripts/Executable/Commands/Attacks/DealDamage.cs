using SerializeReferenceEditor;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

[SRName("Commands/Deal Damage")]
public class DealDamage : AttackCommand
{
    public override bool RequiresMovement => SetMovement();

    public override CardTarget CardTarget => target;

    public CardTarget target;

    bool SetMovement()
    {
        if (CardTarget == CardTarget.Self)
            return false;

        return true;
    }

}
