using SerializeReferenceEditor;
using System;
using System.Collections;
using UnityEngine;

[SRName("Commands/Apply Status Effect")]
public class CMDApplyStatusEffect : StatusCommand
{
    public override StatusEffect Effect => effect.StatusEffect.Clone();

    public override CardTarget CardTarget => target;

    public CardTarget target = CardTarget.Enemy;

    public StatusEffectData effect;


}
