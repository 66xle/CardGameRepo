using UnityEngine;


[CreateAssetMenu(fileName = "Poison", menuName = "StatusEffect/Poison")]

public class StatusPoison : StatusEffect
{
    [Range(0, 1)] public float reduceHealthPercentage;

    public StatusPoison()
    {
        effectName = "Poison";
        effect = Effect.Poison;
        isActiveEffect = true;
    }

    public override void ActivateEffect(Avatar avatar)
    {
        float damage = avatar.maxHealth * reduceHealthPercentage;

        avatar.TakeDamageByStatusEffect(damage);

        SpawnDamageUIPopupGA spawnDamageUIPopupGA = new(avatar, damage, Color.red);
        ActionSystem.Instance.Perform(spawnDamageUIPopupGA);
    }
}
