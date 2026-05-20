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
    public bool OverrideCamera;
    public GameObject FollowTimeline;
    public GameObject AttackTimeline;

    [HideInInspector] public List<AnimationWrapper> AnimationList = new();


    public void UpdateClipData(List<AnimationClipData> animationClipDataList)
    {
        //foreach (AnimationData animationData in Animations)
        //{
        //    if (animationData == null) continue;

        //    animationData.SetDataClipList(animationClipDataList);
        //}
    }

    public void UpdateAnimationList()
    {
        //AnimationList.Clear();

        //foreach (AnimationData animationData in Animations)
        //{
        //    if (animationData == null) continue;

        //    AnimationWrapper wrapper = animationData.GetAnimationWrapper();

        //    if (wrapper == null) continue;

        //    AnimationList.Add(wrapper);
        //}
    }

}
