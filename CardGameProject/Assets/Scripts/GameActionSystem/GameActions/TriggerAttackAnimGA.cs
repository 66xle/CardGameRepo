using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class TriggerAttackAnimGA : GameAction
{
    public Avatar AvatarPlayingCard;
    public string AnimationName;

    public TriggerAttackAnimGA(Avatar avatarPlayingCard, string animationName)
    {
        AvatarPlayingCard = avatarPlayingCard;
        AnimationName = animationName;
    }
}
