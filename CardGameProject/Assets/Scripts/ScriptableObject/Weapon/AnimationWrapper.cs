using UnityEngine;

public class AnimationWrapper
{
    public string AnimationName;
    public float DistanceOffset;

    public AnimationWrapper(string animation, float distance)
    {
        AnimationName = animation;
        DistanceOffset = distance;
    }

    public AnimationWrapper(AnimationClipData data)
    {
        AnimationName = data.Clip.name;
        DistanceOffset = data.DistanceOffset;
    }
}
