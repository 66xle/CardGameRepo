using MyBox;
using SerializeReferenceEditor;

[SRName("Commands/Deal Damage")]
public class CMDDealDamage : AttackCommand
{
    public override bool RequiresMovement => SetMovement();

    public override float Value => value;
    public override bool IsUsingValue => true;

    public override CardTarget CardTarget => target;

    public CardTarget target = CardTarget.Enemy;
    [ConditionalField(false, nameof(CheckCardTarget))] public bool ShouldMove = true;
    public float value;

    public override Executable Clone()
    {
        return new CMDDealDamage
        {
            target = this.target,
            ShouldMove = this.ShouldMove,
            value = this.value,
        };
    }

    bool CheckCardTarget()
    {
        if (CardTarget == CardTarget.Self)
            return false;

        return true;
    }

    bool SetMovement()
    {
        if (CardTarget == CardTarget.Self)
            return false;

        return ShouldMove;
    }
}
