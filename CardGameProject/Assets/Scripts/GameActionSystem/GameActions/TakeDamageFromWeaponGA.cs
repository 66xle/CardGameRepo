
using UnityEngine;
using System.Collections.Generic;

public class TakeDamageFromWeaponGA : GameAction
{
    public Avatar avatarToTakeDamage;
    public CombatStateMachine ctx;
    public float damage;
    public DamageType damageType;
    public CardTarget cardTarget;

    public TakeDamageFromWeaponGA(Avatar avatarToTakeDamage, CombatStateMachine ctx, float damage, DamageType damageType, CardTarget cardTarget)
    {
        this.avatarToTakeDamage = avatarToTakeDamage;
        this.ctx = ctx;
        this.damage = damage;
        this.damageType = damageType;
        this.cardTarget = cardTarget;
    }

    public void ApplyGuardBroken(CombatStateMachine ctx, Avatar avatarOpponent)
    {
        if (avatarOpponent.armourType == ArmourType.Light || avatarOpponent.armourType == ArmourType.None) avatarOpponent.ApplyStatusEffect(ctx.guardBreakLightArmourData.statusEffect.Clone());
        else if (avatarOpponent.armourType == ArmourType.Medium) avatarOpponent.ApplyStatusEffect(ctx.guardBreakMediumArmourData.statusEffect.Clone());
        else if (avatarOpponent.armourType == ArmourType.Heavy) avatarOpponent.ApplyStatusEffect(ctx.guardBreakHeavyArmourData.statusEffect.Clone());
    }
}
