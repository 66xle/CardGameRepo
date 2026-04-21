using MyBox;
using SerializeReferenceEditor;

[SRName("Commands/Draw Card")]
public class CMDDrawCard : DrawCommand
{
    public override bool RequiresMovement => SetMovement();

    public override float Value => value;
    public override bool IsUsingValue => true;

    public override CardTarget CardTarget => target;

    public float value;
    [ReadOnly] public CardTarget target = CardTarget.Self;

    public override Executable Clone()
    {
        return new CMDDrawCard
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
