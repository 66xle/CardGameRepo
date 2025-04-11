using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;

[CreateAssetMenu(fileName = "GuardBroken", menuName = "StatusEffect/GuardBroken")]
public class StatusGuardBroken : StatusEffect
{
    public int numberOfHitsToRecover;
    [Range(0, 1)] public float extraDamagePercentage;
    [SerializeField] bool removeEffectNextTurn;

    public StatusGuardBroken()
    {
        effectName = "Guard Broken";
        effect = Effect.GuardBroken;
        removeEffectNextTurn = true;
    }

    public override StatusEffect Clone()
    {
        return (StatusEffect)this.MemberwiseClone();
    }

    public override bool ShouldRemoveEffectNextTurn()
    {
        return removeEffectNextTurn;
    }

    public override void SetRemoveEffectNextTurn(bool value)
    {
        removeEffectNextTurn = value;
    }

    public override void OnApply(Avatar avatar)
    {
        avatar.animator.SetBool("isStunned", true);
        base.OnApply(avatar);
    }

    public override void OnRemoval(Avatar avatar)
    {
        avatar.animator.SetBool("isStunned", false);
    }
}
