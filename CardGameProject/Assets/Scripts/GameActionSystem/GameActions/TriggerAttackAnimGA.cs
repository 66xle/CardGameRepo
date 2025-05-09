using UnityEngine;

public class TriggerAttackAnimGA : GameAction
{
    public Avatar AvatarPlayingCard;
    public WeaponType WeaponType;

    public TriggerAttackAnimGA(Avatar avatarPlayingCard, WeaponType weaponType)
    {
        AvatarPlayingCard = avatarPlayingCard;
        WeaponType = weaponType;
    }
}
