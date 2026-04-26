using SerializeReferenceEditor;

[SRName("Commands/Deal Guard Damage")]
public class CMDDealGuardDamage : GuardCommand
{
    public override bool RequiresMovement => SetMovement();

    public override float Value => value;
    public override bool IsUsingValue => true;

    public override CardTarget CardTarget => target;

    public float value;
    public CardTarget target = CardTarget.Enemy;

    public override Executable Clone()
    {
        return new CMDDealGuardDamage
        {
            target = this.target,
            value = this.value,
        };
    }

    bool SetMovement()
    {
        if (CardTarget == CardTarget.Self)
            return false;

        return true;
    }
}
