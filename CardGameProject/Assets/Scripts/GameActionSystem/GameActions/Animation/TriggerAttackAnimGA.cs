using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Playables;

public class TriggerAttackAnimGA : GameAction
{
    public Avatar AvatarPlayingCard;
    public string AnimationName;
    public GameObject AttackTimeline;
    public AudioResource AudioResource;
    public bool IsAttackAnimation;

    public TriggerAttackAnimGA(Avatar avatarPlayingCard, string animationName, GameObject attackTimeline, AudioResource audioResource, bool isAttackAnimation = true)
    {
        AvatarPlayingCard = avatarPlayingCard;
        AnimationName = animationName;
        AttackTimeline = attackTimeline;
        AudioResource = audioResource;
        IsAttackAnimation = isAttackAnimation;
    }
}
