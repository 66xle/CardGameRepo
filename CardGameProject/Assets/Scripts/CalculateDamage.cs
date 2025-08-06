using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public static class CalculateDamage
{

    public static float GetDamage(float avatarAttack, float weaponAttack, Avatar avatarTakeDamage, float multiplier)
    {
        float damage = Mathf.Ceil((avatarAttack + weaponAttack) * multiplier);

        if (avatarTakeDamage != null)
        {
            damage = Mathf.Ceil(avatarTakeDamage.ApplyAdditionalDmgCheck(damage));

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
}
