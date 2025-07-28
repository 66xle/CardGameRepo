using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Playables;

public class TriggerAttackAnimGA : GameAction
{
    public Avatar AvatarPlayingCard;
    public string AnimationName;
    public PlayableAsset AttackTimeline;

    public TriggerAttackAnimGA(Avatar avatarPlayingCard, string animationName, PlayableAsset attackTimeline)
    {
        AvatarPlayingCard = avatarPlayingCard;
        AnimationName = animationName;
        AttackTimeline = attackTimeline;
    }
}
