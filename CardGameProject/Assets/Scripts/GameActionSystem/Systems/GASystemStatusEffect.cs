using System.Collections;
using UnityEngine;

public class GASystemStatusEffect : MonoBehaviour
{
    private void OnEnable()
    {
        ActionSystem.AttachPerformer<GAApplyStatusEffect>(ApplyStatusEffectPerformer);
    }

    private void OnDisable()
    {
        ActionSystem.DetachPerformer<GAApplyStatusEffect>();
    }

    private IEnumerator ApplyStatusEffectPerformer(GAApplyStatusEffect applyStatusEffectGA)
    {
        Avatar avatarToApply = applyStatusEffectGA.AvatarToApply;
        StatusEffect statusEffect = applyStatusEffectGA.StatusEffect;

        avatarToApply.ApplyStatusEffect(statusEffect);

        yield return null;
    }
}
