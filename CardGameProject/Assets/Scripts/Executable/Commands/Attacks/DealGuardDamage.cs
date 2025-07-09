using SerializeReferenceEditor;
using UnityEngine;

[SRName("Commands/Deal Guard Damage")]
public class DealGuardDamage : ReduceGuardCommand
{
    public override bool RequiresMovement => SetMovement();

    public override float Value => value;
    public override bool IsUsingValue => true;

    public override CardTarget CardTarget => target;

    public CardTarget target = CardTarget.Enemy;
    public float value;

    bool SetMovement()
    {
        if (CardTarget == CardTarget.Self)
            return false;

        return true;
    }
}
