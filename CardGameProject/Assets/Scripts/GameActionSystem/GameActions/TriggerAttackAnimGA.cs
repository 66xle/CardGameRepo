using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class TriggerAttackAnimGA : GameAction
{
    public Avatar AvatarPlayingCard;
    public List<string> AnimationList;

    public TriggerAttackAnimGA(Avatar avatarPlayingCard, List<string> animationList)
    {
        AvatarPlayingCard = avatarPlayingCard;
        AnimationList = animationList;
    }
}
