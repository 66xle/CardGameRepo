using UnityEngine;

public class SpawnDamageUIPopupGA : GameAction
{
    public Avatar avatarTakingDamage;
    public float damage;
    public Color color;

    public SpawnDamageUIPopupGA(Avatar avatarTakingDamage, float damage, Color color)
    {
        this.avatarTakingDamage = avatarTakingDamage;
        this.damage = damage;
        this.color = color;
    }
}
