using System;
using System.Collections.Generic;
using SerializeReferenceEditor;
using UnityEngine;
using UnityEngine.Audio;

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
public class CardAnimationData
{
    public Card Card;
    public int CardAmount = 1;
    [HideInInspector] public List<string> EnabledVariantID = new();

    public bool SkipAnimation;
    public AnimationClip Animation;
    public AudioResource AudioResource;
    public bool OverrideDistanceOffset;
    public float DistanceOffset;
    public bool OverrideMoveTime;
    public float MoveTime;
    public bool OverrideCameraFollow;
    public bool OverrideCameraAttack;
    public GameObject FollowTimeline;
    public GameObject AttackTimeline;

    [HideInInspector] public List<AnimationClipData> AnimationClipDataList;
    [HideInInspector] public List<AnimationWrapper> AnimationList = new();


    public void UpdateClipData(List<AnimationClipData> animationClipDataList)
    {
        AnimationClipDataList = animationClipDataList;
    }

    public AnimationWrapper GetAnimationWrapper()
    {
        if (!SkipAnimation && Animation == null) return null;

        if (SkipAnimation)
            return new AnimationWrapper(true);


        if (Animation.name == AttackType.None.ToString()) return null;

        char split = '_';
        string[] stringSplit = Animation.name.Split(split);

        if (stringSplit[0] == "") return null;

        if (!int.TryParse(stringSplit[0], out int result))
            return null;

        float distance = AnimationClipDataList[result].DistanceOffset;
        float moveTime = 0f;

        GameObject followTimeline = null;
        GameObject attackTimeline = null;

        if (OverrideDistanceOffset)
            distance = DistanceOffset;

        if (OverrideMoveTime)
            moveTime = MoveTime;

        if (OverrideCameraAttack)
            attackTimeline = AttackTimeline;

        if (OverrideCameraFollow)
            followTimeline = FollowTimeline;

        return new AnimationWrapper(stringSplit[1], distance, followTimeline, attackTimeline, AudioResource, moveTime);
    }

    public void UpdateAnimationList()
    {
        AnimationList.Clear();

        AnimationWrapper wrapper = GetAnimationWrapper();
        AnimationList.Add(wrapper);

        
        //foreach (AnimationData animationData in Animations)
        //{
        //    if (animationData == null) continue;

        //    AnimationWrapper wrapper = animationData.GetAnimationWrapper();

        //    if (wrapper == null) continue;

        //    AnimationList.Add(wrapper);
        //}
    }

}
