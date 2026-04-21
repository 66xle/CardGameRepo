using SerializeReferenceEditor;

[SRName("Commands/Apply Status Effect")]
public class CMDApplyStatusEffect : StatusCommand
{
    public override StatusEffect Effect => effect.StatusEffect.Clone();

    public override CardTarget CardTarget => target;

    public CardTarget target = CardTarget.Enemy;

    public StatusEffectData effect;

    public override Executable Clone()
    {
        return new CMDApplyStatusEffect
        {
            target = this.target,
            effect = this.effect, // or deep clone if needed
        };
    }
}
