using UnityEngine;

public class SpawnDamageUIPopupGA : GameAction
{
    public Avatar AvatarTakingDamage;
    public string Text;
    public Color Color;
    public bool IsStatusEffect;

    public SpawnDamageUIPopupGA(Avatar avatarTakingDamage, string text, Color color, bool isStatusEffect = false)
    {
        AvatarTakingDamage = avatarTakingDamage;
        Text = text;
        Color = color;
        IsStatusEffect = isStatusEffect;
    }
}
