using UnityEngine;

public class GAGainHealth : GameAction
{
    public Avatar AvatarTarget;
    public int HealAmount;

    public GAGainHealth(Avatar target, int healAmount)
    {
        AvatarTarget = target;
        HealAmount = healAmount;
    }
}
