using UnityEngine;

public class TakeGuardDamageGA : GameAction
{
    public Avatar AvatarToTakeDamage;
    public int GuardDamage;
    public CardTarget CardTarget;

    public TakeGuardDamageGA(Avatar avatarToTakeDamage, float guardDamage, CardTarget cardTarget)
    {
        AvatarToTakeDamage = avatarToTakeDamage;
        GuardDamage = (int)guardDamage;
        CardTarget = cardTarget;
    }
}
