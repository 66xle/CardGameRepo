using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;
using System;

public class StatusBleed : StatusEffect
{
    [Range(0, 1)] public float reduceHealthPercentage;
    [ReadOnly] public int stacks;

    public StatusBleed()
    {
        effectName = "Bleed";
        effect = Effect.Bleed;
        isActiveEffect = true;
    }

    public override int GetStacks()
    {
        return stacks;
    }

    public override void OnApply(Avatar avatar)
    {
        stacks++;
    }

    public override void ActivateEffect(Avatar avatar)
    {
        float damage = avatar.maxHealth * reduceHealthPercentage * stacks;

        avatar.TakeDamageByStatusEffect(damage);

        SpawnDamageUIPopupGA spawnDamageUIPopupGA = new(avatar, damage, Color.red);
        ActionSystem.Instance.Perform(spawnDamageUIPopupGA);
    }
}
