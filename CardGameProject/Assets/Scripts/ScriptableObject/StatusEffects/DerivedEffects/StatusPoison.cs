using UnityEngine;


[CreateAssetMenu(fileName = "Poison", menuName = "StatusEffect/Poison")]

public class StatusPoison : StatusEffect
{
    [Range(0, 1)] public float ReduceHealthPercentage;

    public StatusPoison()
    {
        EffectName = "Poison";
        Effect = Effect.Poison;
        IsActiveEffect = true;
    }

    public override StatusEffect Clone()
    {
        return (StatusEffect)this.MemberwiseClone();
    }

    public override void ActivateEffect(Avatar avatar)
    {
        float damage = avatar.MaxHealth * ReduceHealthPercentage;

        avatar.TakeDamageByStatusEffect(damage);

        SpawnDamageUIPopupGA spawnDamageUIPopupGA = new(avatar, damage, Color.red);
        ActionSystem.Instance.Perform(spawnDamageUIPopupGA);
    }

    public override float GetDataPopup()
    {
        return ReduceHealthPercentage * 100;
    }
}
