using System;
using System.Collections.Generic;
using MyBox;
using SerializeReferenceEditor;
using UnityEngine.Playables;

public enum AttackType
{
    None
}


public enum Boolean
{
    True,
    False
}

[Serializable]
[SRHidden]
public class AnimationData
{
    public virtual bool EnableAnimation { get; set; }
    public virtual string Animation { get; set; }
    public virtual Boolean OverrideDistanceOffset { get; set; }
    public virtual float DistanceOffset { get; set; }
    public virtual Boolean OverrideCamera { get; set; }
    public virtual PlayableAsset FollowTimeline { get; set; }
    public virtual PlayableAsset AttackTimeline { get; set; }

    public virtual void SetDataClipList(List<AnimationClipData> dataClipList) { }

    public virtual string[] GetAnimationNames()
    {
        return null;
    }

    public virtual AnimationWrapper GetAnimationWrapper()
    {
        return null;
    }

    

    public bool OverrideDistance()
    {
        if (OverrideDistanceOffset == Boolean.False) return false;

        if (Animation == AttackType.None.ToString()) return false;

        return true;
    }

    public bool OverrideVirtualCamera()
    {
        if (OverrideCamera == Boolean.False)
            return false;

        return true;
    }
}
