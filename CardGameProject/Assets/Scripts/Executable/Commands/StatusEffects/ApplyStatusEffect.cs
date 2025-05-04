using SerializeReferenceEditor;
using System;
using System.Collections;
using UnityEngine;

[SRName("Commands/Apply Status Effect")]
public class ApplyStatusEffect : StatusCommand
{
    public override StatusEffect Effect => effect.statusEffect;

    public override CardTarget CardTarget => target;

    public CardTarget target = CardTarget.Enemy;

    public StatusEffectData effect;


}
