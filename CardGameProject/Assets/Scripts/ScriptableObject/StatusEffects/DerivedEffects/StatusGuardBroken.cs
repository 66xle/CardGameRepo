using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;

[CreateAssetMenu(fileName = "GuardBroken", menuName = "StatusEffect/GuardBroken")]
public class StatusGuardBroken : StatusEffect
{
    [Range(0, 1)] public float ExtraDamagePercentage;
    [SerializeField] bool RemoveEffectNextTurn;

    public StatusGuardBroken()
    {
        EffectName = "Guard Broken";
        Effect = Effect.GuardBroken;
        RemoveEffectNextTurn = true;
    }

    public override StatusEffect Clone()
    {
        return (StatusEffect)this.MemberwiseClone();
    }

    public override bool ShouldRemoveEffectNextTurn()
    {
        return RemoveEffectNextTurn;
    }

    public override void SetRemoveEffectNextTurn(bool value)
    {
        RemoveEffectNextTurn = value;
    }

    public override void OnApply(Avatar avatar)
    {
        avatar.Animator.SetBool("isStunned", true);
        base.OnApply(avatar);
    }

    public override void OnRemoval(Avatar avatar)
    {
        avatar.Animator.SetBool("isStunned", false);
    }
}
