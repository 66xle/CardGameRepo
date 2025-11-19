using System.Collections;
using UnityEngine;

public class StatusEffectSystem : MonoBehaviour
{
    private void OnEnable()
    {
        ActionSystem.AttachPerformer<ApplyStatusEffectGA>(ApplyStatusEffectPerformer);
    }

    private void OnDisable()
    {
        ActionSystem.DetachPerformer<ApplyStatusEffectGA>();
    }

    private IEnumerator ApplyStatusEffectPerformer(ApplyStatusEffectGA applyStatusEffectGA)
    {
        Avatar avatarToApply = applyStatusEffectGA.AvatarToApply;
        StatusEffect statusEffect = applyStatusEffectGA.StatusEffect;

        avatarToApply.ApplyStatusEffect(statusEffect);

        yield return null;
    }
}
