using MyBox;
using UnityEngine.Playables;

[System.Serializable]
public class AnimationWrapper
{
    public string AnimationName;
    public float DistanceOffset;
    public PlayableAsset FollowTimeline;
    public PlayableAsset AttackTimeline;
    public AudioType AudioType;

    public AnimationWrapper(string animation, float distance, PlayableAsset followTimeline, PlayableAsset attackTimeline, AudioType audioType)
    {
        AnimationName = animation;
        DistanceOffset = distance;
        FollowTimeline = followTimeline;
        AttackTimeline = attackTimeline;
        AudioType = audioType;
    }

    public AnimationWrapper(AnimationClipData data)
    {
        AnimationName = data.Clip.name;
        DistanceOffset = data.DistanceOffset;
    }
}
