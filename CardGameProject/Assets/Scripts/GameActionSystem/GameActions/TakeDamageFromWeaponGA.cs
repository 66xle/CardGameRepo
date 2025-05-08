
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

    public void ReduceHitToRecover(Avatar avatarOpponent)
    {
        for (int i = avatarOpponent.ListOfEffects.Count - 1; i >= 0; i--)
        {
            if (avatarOpponent.ListOfEffects[i].effect != Effect.GuardBroken)
                continue;

            avatarOpponent.ListOfEffects[i].currentTurnsRemaning--;
            if (avatarOpponent.ListOfEffects[i].currentTurnsRemaning <= 0)
            {
                avatarOpponent.RecoverGuardBreak();
                avatarOpponent.ListOfEffects[i].OnRemoval(avatarOpponent);
                avatarOpponent.ListOfEffects.RemoveAt(i);
            }
        }
    }

    public void ApplyGuardBroken(CombatStateMachine ctx, Avatar avatarOpponent)
    {
        if (avatarOpponent.ArmourType == ArmourType.Light || avatarOpponent.ArmourType == ArmourType.None) avatarOpponent.ApplyStatusEffect(ctx.guardBreakLightArmourData.statusEffect.Clone());
        else if (avatarOpponent.ArmourType == ArmourType.Medium) avatarOpponent.ApplyStatusEffect(ctx.guardBreakMediumArmourData.statusEffect.Clone());
        else if (avatarOpponent.ArmourType == ArmourType.Heavy) avatarOpponent.ApplyStatusEffect(ctx.guardBreakHeavyArmourData.statusEffect.Clone());
    }
}
