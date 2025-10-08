using System;
using System.Collections.Generic;
using MyBox;
using SerializeReferenceEditor;
using UnityEngine;
using UnityEngine.Playables;

[Serializable]
[SRName("Armour")]
public class ArmourAnimationData : AnimationData
{
    public override bool EnableAnimation => _skipAnimation;
    public override string Animation => _animation.name;
    public override Boolean OverrideDistanceOffset => _overrideDistanceOffset;
    public override float DistanceOffset => _distanceOffset;
    public override Boolean OverrideMoveTime => _overrideMoveTime;
    public override float MoveTime => _moveTime;
    public override Boolean OverrideCamera => _overrideCamera;
    public override PlayableAsset FollowTimeline => _followTimeline;
    public override PlayableAsset AttackTimeline => _attackTimeline;


    public bool _skipAnimation = false;
    [ConditionalField(false, nameof(ShowAnimation))] public AnimationClip _animation;
    [ConditionalField(false, nameof(AnimationClip))] public bool IsAttackAnimation = false;
    [ConditionalField(false, nameof(AnimationClip))] public AudioType AudioType;

    [ConditionalField(false, nameof(AnimationClip))] public Boolean _overrideDistanceOffset = Boolean.False;
    [ConditionalField(false, nameof(OverrideDistance))] public float _distanceOffset = 0;

    [ConditionalField(nameof(_animation), true, AttackType.None)] public Boolean _overrideMoveTime = Boolean.False;
    [ConditionalField(false, nameof(OverrideMove))] public float _moveTime = 0;

    [ConditionalField(false, nameof(AnimationClip))] public Boolean _overrideCamera = Boolean.False;
    [ConditionalField(false, nameof(OverrideVirtualCamera))] public PlayableAsset _followTimeline;
    [ConditionalField(false, nameof(OverrideVirtualCamera))] public PlayableAsset _attackTimeline;

    public override AnimationWrapper GetAnimationWrapper()
    {
        if (!_skipAnimation && _animation == null) return null;

        float distance = 0f;
        float moveTime = 0f;

        PlayableAsset followTimeline = null;
        PlayableAsset attackTimeline = null;

        if (OverrideDistanceOffset == Boolean.True)
            distance = DistanceOffset;

        if (OverrideMoveTime == Boolean.True)
            moveTime = MoveTime;

        if (OverrideCamera == Boolean.True)
        {
            followTimeline = FollowTimeline;
            attackTimeline = AttackTimeline;
        }

        if (_skipAnimation)
            return new AnimationWrapper(_skipAnimation);

        return new AnimationWrapper(Animation, distance, followTimeline, attackTimeline, AudioType, moveTime, IsAttackAnimation);
    }

    public bool AnimationClip()
    {
        if (_animation == null) return false;

        return true;
    }

    public bool ShowAnimation()
    {
        if (!_skipAnimation) return true;

        _animation = null;
        return false;
    }
}
