using MyBox;
using SerializeReferenceEditor;

[SRName("Commands/Gain Block")]
public class GainBlock : BlockCommand
{
    public override bool RequiresMovement => SetMovement();

    public override float Value => value;
    public override bool IsUsingValue => true;

    public override CardTarget CardTarget => target;

    [ReadOnly] public CardTarget target = CardTarget.Self;
    public float value;

    bool SetMovement()
    {
        return false;
    }
}
