using UnityEngine;

public class GAApplyStatusEffect : GameAction
{
    public Avatar AvatarToApply;
    public StatusEffect StatusEffect;

    public GAApplyStatusEffect(Avatar avatarToApply, StatusEffect statusEffect)
    {
        AvatarToApply = avatarToApply;
        StatusEffect = statusEffect;
    }
}
