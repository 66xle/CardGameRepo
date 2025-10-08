using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public static class CalculateDamage
{

    public static float GetDamage(float avatarAttack, float weaponAttack, Avatar avatarTakeDamage, Avatar avatarPlayingCard, float multiplier)
    {
        float damage = Mathf.Ceil((avatarAttack + weaponAttack) * multiplier);

        if (avatarTakeDamage != null && avatarPlayingCard != null)
        {
            damage = Mathf.Ceil(avatarTakeDamage.ApplyAdditionalDmgCheck(damage));

            damage = Mathf.Ceil(avatarPlayingCard.ApplyBuffDmgCheck(damage));

            if (avatarTakeDamage.IsInCounterState)
                damage = Mathf.Ceil(damage / 2f);

            damage -= Mathf.Ceil(damage * CalculateDefencePercentage(avatarTakeDamage.Defence, avatarTakeDamage.DefencePercentage));
        }

        return damage;
    }

    private static float CalculateDefencePercentage(float def, float defPer)
    {
        return def / (def + defPer);
    }


    public static float GetBlock(float defence, float multiplier, float blockScale)
    {
        float block = Mathf.Sqrt(defence * blockScale) * multiplier;

        return Mathf.Ceil(block);
    }

    public static int GetHealAmount(float health, float multiplier)
    {
        float heal = health * multiplier;

        return (int)Mathf.Ceil(heal);
    }
}
