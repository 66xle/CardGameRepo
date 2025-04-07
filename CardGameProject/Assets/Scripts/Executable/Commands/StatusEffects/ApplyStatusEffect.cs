using SerializeReferenceEditor;
using System;
using System.Collections;
using UnityEngine;

public class ApplyStatusEffect : StatusCommand
{
    public override StatusEffect Effect => effect.statusEffect;

    public override CardTarget CardTarget => target;

    public CardTarget target;

    public StatusEffectData effect;


}
