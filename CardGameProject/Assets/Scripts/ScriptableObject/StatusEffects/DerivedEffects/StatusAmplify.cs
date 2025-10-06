using UnityEngine;
using MyBox;
using System;

public class StatusAmplify : StatusEffect
{
    [Range(0, 1)] public float AttackIncreasePercentage;

    public StatusAmplify()
    {
        EffectName = "Amplify";
        Effect = Effect.Amplify;
        IsActiveEffect = false;
    }

    public override StatusEffect Clone()
    {
        return (StatusEffect)this.MemberwiseClone();
    }

    public override void OnApply(Avatar avatar)
    {
        SpawnDamageUIPopupGA spawnDamageUIPopupGA = new(avatar, "Amplify", new Color32(255, 165, 0, 255), true); // Orange color
        ActionSystem.Instance.Perform(spawnDamageUIPopupGA);

        base.OnApply(avatar);
    }

    public override float GetDataPopup()
    {
        return AttackIncreasePercentage * 100;
    }
}
