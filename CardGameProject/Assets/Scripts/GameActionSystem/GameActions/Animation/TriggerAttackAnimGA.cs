using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Playables;

public class TriggerAttackAnimGA : GameAction
{
    public Avatar AvatarPlayingCard;
    public string AnimationName;
    public PlayableAsset AttackTimeline;
    public AudioResource AudioResource;
    public bool EnableAttackCamera;

    public TriggerAttackAnimGA(Avatar avatarPlayingCard, string animationName, PlayableAsset attackTimeline, AudioResource audioResource, bool enableAttackCamera = true)
    {
        AvatarPlayingCard = avatarPlayingCard;
        AnimationName = animationName;
        AttackTimeline = attackTimeline;
        AudioResource = audioResource;
        EnableAttackCamera = enableAttackCamera;
    }
}
