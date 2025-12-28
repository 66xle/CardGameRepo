
using UnityEngine;
using System.Collections.Generic;

public class GATakeDamageFromWeapon : GameAction
{
    public Avatar AvatarToTakeDamage;
    public float Damage;
    public DamageType DamageType;
    public CardTarget CardTarget;

    public GATakeDamageFromWeapon(Avatar avatarToTakeDamage, float damage, DamageType damageType, CardTarget cardTarget)
    {
        AvatarToTakeDamage = avatarToTakeDamage;
        Damage = damage;
        DamageType = damageType;
        CardTarget = cardTarget;
    }
}
