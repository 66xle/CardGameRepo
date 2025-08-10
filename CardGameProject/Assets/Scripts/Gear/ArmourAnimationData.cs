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
    public override string Animation => _animation.name;
    public override Boolean OverrideDistanceOffset => _overrideDistanceOffset;
    public override float DistanceOffset => _distanceOffset;
    public override Boolean OverrideCamera => _overrideCamera;
    public override PlayableAsset FollowTimeline => _followTimeline;
    public override PlayableAsset AttackTimeline => _attackTimeline;


    public AnimationClip _animation;
    [ConditionalField(false, nameof(AnimationClip))] public Boolean _overrideDistanceOffset = Boolean.False;
    [ConditionalField(false, nameof(OverrideDistance))] public float _distanceOffset = 0;
    [ConditionalField(false, nameof(AnimationClip))] public Boolean _overrideCamera = Boolean.False;
    [ConditionalField(false, nameof(OverrideVirtualCamera))] public PlayableAsset _followTimeline;
    [ConditionalField(false, nameof(OverrideVirtualCamera))] public PlayableAsset _attackTimeline;

    public override AnimationWrapper GetAnimationWrapper()
    {
        if (_animation == null) return null;

        float distance = 0f;

        PlayableAsset followTimeline = null;
        PlayableAsset attackTimeline = null;

        if (OverrideDistanceOffset == Boolean.True)
            distance = DistanceOffset;

        if (OverrideCamera == Boolean.True)
        {
            followTimeline = FollowTimeline;
            attackTimeline = AttackTimeline;
        }

        return new AnimationWrapper(Animation, distance, followTimeline, attackTimeline);
    }

    public bool AnimationClip()
    {
        if (_animation == null) return false;

        return true;
    }
}
