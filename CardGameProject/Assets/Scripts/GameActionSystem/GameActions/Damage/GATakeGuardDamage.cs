using UnityEngine;

public class GATakeGuardDamage : GameAction
{
    public Avatar AvatarToTakeDamage;
    public int GuardDamage;
    public CardTarget CardTarget;

    public GATakeGuardDamage(Avatar avatarToTakeDamage, float guardDamage, CardTarget cardTarget)
    {
        AvatarToTakeDamage = avatarToTakeDamage;
        GuardDamage = (int)guardDamage;
        CardTarget = cardTarget;
    }
}
