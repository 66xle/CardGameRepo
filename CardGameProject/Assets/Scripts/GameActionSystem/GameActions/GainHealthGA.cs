using UnityEngine;

public class GainHealthGA : GameAction
{
    public Avatar AvatarTarget;
    public int HealAmount;

    public GainHealthGA(Avatar target, int healAmount)
    {
        AvatarTarget = target;
        HealAmount = healAmount;
    }
}
