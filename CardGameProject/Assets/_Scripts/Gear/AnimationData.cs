using SerializeReferenceEditor;
using System;
using System.Collections.Generic;
using UnityEngine;

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
public class AnimationData
{
    public bool EnableAnimation;
    public virtual string Animation { get; set; }
    public virtual Boolean OverrideDistanceOffset { get; set; }
    public virtual float DistanceOffset { get; set; }
    public virtual Boolean OverrideMoveTime { get; set; }
    public virtual float MoveTime { get; set; }
    public virtual Boolean OverrideCamera { get; set; }
    public virtual GameObject FollowTimeline { get; set; }
    public virtual GameObject AttackTimeline { get; set; }

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

    public bool OverrideMove()
    {
        if (OverrideMoveTime == Boolean.False) return false;

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
