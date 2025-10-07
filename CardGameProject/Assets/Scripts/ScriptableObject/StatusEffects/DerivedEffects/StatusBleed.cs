using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;
using System;

public class StatusBleed : StatusEffect
{
    [Range(0, 1)] public float ReduceHealthPercentage;
    [ReadOnly] public int Stacks;

    public StatusBleed()
    {
        EffectName = "Bleed";
        Effect = Effect.Bleed;
        IsActiveEffect = true;
    }

    public override StatusEffect Clone()
    {
        return (StatusEffect)this.MemberwiseClone();
    }

    public override int GetStacks()
    {
        return Stacks;
    }

    public override void OnApply(Avatar avatar)
    {
        Stacks++;

        SpawnDamageUIPopupGA spawnDamageUIPopupGA = new(avatar, "Bleed", Color.red, true);
        ActionSystem.Instance.Perform(spawnDamageUIPopupGA);

        base.OnApply(avatar);
    }

    public override void ActivateEffect(Avatar avatar)
    {
        float damage = Mathf.Ceil(avatar.MaxHealth * ReduceHealthPercentage * Stacks);

        avatar.TakeDamageByStatusEffect(damage);

        SpawnDamageUIPopupGA spawnDamageUIPopupGA = new(avatar, damage.ToString(), Color.red);
        ActionSystem.Instance.Perform(spawnDamageUIPopupGA);
    }

    public override float GetDataPopup()
    {
        return ReduceHealthPercentage * 100;
    }
}
