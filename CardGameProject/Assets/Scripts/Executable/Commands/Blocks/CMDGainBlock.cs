using MyBox;
using SerializeReferenceEditor;

[SRName("Commands/Gain Block")]
public class CMDGainBlock : BlockCommand
{
    public override bool RequiresMovement => SetMovement();

    public override float Value => value;
    public override bool IsUsingValue => true;

    public override CardTarget CardTarget => target;

    public float value;
    [ReadOnly] public CardTarget target = CardTarget.Self;

    public override Executable Clone()
    {
        return new CMDGainBlock
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
