
using UnityEngine;
using System.Collections.Generic;

public class TakeDamageFromWeaponGA : GameAction
{
    public Avatar AvatarToTakeDamage;
    public float Damage;
    public DamageType DamageType;
    public CardTarget CardTarget;

    public TakeDamageFromWeaponGA(Avatar avatarToTakeDamage, float damage, DamageType damageType, CardTarget cardTarget)
    {
        AvatarToTakeDamage = avatarToTakeDamage;
        Damage = damage;
        DamageType = damageType;
        CardTarget = cardTarget;
    }
}
