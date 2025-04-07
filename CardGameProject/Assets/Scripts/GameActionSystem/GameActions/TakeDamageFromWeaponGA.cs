
using UnityEngine;
using System.Collections.Generic;

public class TakeDamageFromWeaponGA : GameAction
{
    public Avatar avatarToTakeDamage;
    public CombatStateMachine ctx;
    public float damage;
    public DamageType damageType;

    public TakeDamageFromWeaponGA(Avatar avatarToTakeDamage, CombatStateMachine ctx, float damage, DamageType damageType)
    {
        this.avatarToTakeDamage = avatarToTakeDamage;
        this.ctx = ctx;
        this.damage = damage;
        this.damageType = damageType;
    }

    public void ApplyGuardBroken(CombatStateMachine ctx, Avatar avatarOpponent)
    {
        if (avatarOpponent.armourType == ArmourType.Light || avatarOpponent.armourType == ArmourType.None) avatarOpponent.ApplyStatusEffect(ctx.guardBreakLightArmourData.statusEffect);
        else if (avatarOpponent.armourType == ArmourType.Medium) avatarOpponent.ApplyStatusEffect(ctx.guardBreakMediumArmourData.statusEffect);
        else if (avatarOpponent.armourType == ArmourType.Heavy) avatarOpponent.ApplyStatusEffect(ctx.guardBreakHeavyArmourData.statusEffect);
    }
}
