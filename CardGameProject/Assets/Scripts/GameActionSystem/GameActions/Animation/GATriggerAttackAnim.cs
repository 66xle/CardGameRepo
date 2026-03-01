using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Playables;

public class GATriggerAttackAnim : GameAction
{
    public Avatar AvatarPlayingCard;
    public string AnimationName;
    public GameObject AttackTimeline;
    public AudioResource AudioResource;
    public bool IsAttackAnimation;

    public GATriggerAttackAnim(Avatar avatarPlayingCard, string animationName, GameObject attackTimeline, AudioResource audioResource, bool isAttackAnimation = true)
    {
        AvatarPlayingCard = avatarPlayingCard;
        AnimationName = animationName;
        AttackTimeline = attackTimeline;
        AudioResource = audioResource;
        IsAttackAnimation = isAttackAnimation;
    }
}
