using SerializeReferenceEditor;
using MyBox;
using UnityEditor;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.XR;

[SRName("Commands/Deal Damage")]
public class DealDamage : AttackCommand
{
    public override bool RequiresMovement => SetMovement();

    public override float Value => value; 
    public override bool IsUsingValue => true;

    public override CardTarget CardTarget => target;

    public CardTarget target;
    public float value;

    bool SetMovement()
    {
        if (CardTarget == CardTarget.Self)
            return false;

        return true;
    }
}
