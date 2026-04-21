using MyBox;
using SerializeReferenceEditor;
using UnityEngine;

[SRName("Commands/Gain Health")]
public class CMDGainHealth : HealCommand
{
    public override bool RequiresMovement => SetMovement();

    public override float Value => value;
    public override bool IsUsingValue => true;

    public override CardTarget CardTarget => target;

    [Range(0, 1)] public float value;
    [ReadOnly] public CardTarget target = CardTarget.Self;

    public override Executable Clone()
    {
        return new CMDGainHealth
        {
            target = this.target,
            value = this.value,
        };
    }
    bool SetMovement()
    {
        return false;
    }
}
