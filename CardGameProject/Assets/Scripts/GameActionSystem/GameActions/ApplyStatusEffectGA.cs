using UnityEngine;

public class ApplyStatusEffectGA : GameAction
{
    public Avatar AvatarToApply;
    public StatusEffect StatusEffect;

    public ApplyStatusEffectGA(Avatar avatarToApply, StatusEffect statusEffect)
    {
        AvatarToApply = avatarToApply;
        StatusEffect = statusEffect;
    }
}
