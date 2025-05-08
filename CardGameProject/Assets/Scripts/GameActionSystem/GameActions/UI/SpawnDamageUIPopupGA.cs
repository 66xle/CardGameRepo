using UnityEngine;

public class SpawnDamageUIPopupGA : GameAction
{
    public Avatar AvatarTakingDamage;
    public float Damage;
    public Color Color;

    public SpawnDamageUIPopupGA(Avatar avatarTakingDamage, float damage, Color color)
    {
        AvatarTakingDamage = avatarTakingDamage;
        Damage = damage;
        Color = color;
    }
}
