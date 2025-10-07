using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Playables;

public class TriggerAttackAnimGA : GameAction
{
    public Avatar AvatarPlayingCard;
    public string AnimationName;
    public PlayableAsset AttackTimeline;
    public AudioType AudioType;
    public bool EnableAttackCamera;

    public TriggerAttackAnimGA(Avatar avatarPlayingCard, string animationName, PlayableAsset attackTimeline, AudioType audioType, bool enableAttackCamera = true)
    {
        AvatarPlayingCard = avatarPlayingCard;
        AnimationName = animationName;
        AttackTimeline = attackTimeline;
        AudioType = audioType;
        EnableAttackCamera = enableAttackCamera;
    }
}
