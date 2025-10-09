using MyBox;
using UnityEngine.Playables;
using UnityEngine.Audio;
using UnityEngine;

[System.Serializable]
public class AnimationWrapper
{
    public string AnimationName;
    public float DistanceOffset;
    public GameObject FollowTimeline;
    public GameObject AttackTimeline;
    public AudioResource AudioResource;
    public bool SkipAnimation;
    public float MoveTime;
    public bool IsAttackAnimation;

    public AnimationWrapper(string animation, float distance, GameObject followTimeline, GameObject attackTimeline, AudioResource audioResource, float moveTime, bool isAttackAnimation = true)
    {
        AnimationName = animation;
        DistanceOffset = distance;
        FollowTimeline = followTimeline;
        AttackTimeline = attackTimeline;
        AudioResource = audioResource;
        SkipAnimation = false;
        MoveTime = moveTime;
        IsAttackAnimation = isAttackAnimation;
    }

    public AnimationWrapper(bool skipAnimation)
    {
        SkipAnimation = skipAnimation;
    }

    public AnimationWrapper(AnimationClipData data)
    {
        AnimationName = data.Clip.name;
        DistanceOffset = data.DistanceOffset;
    }
}
