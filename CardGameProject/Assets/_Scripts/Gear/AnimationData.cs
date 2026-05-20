using System;
using System.Collections.Generic;
using SerializeReferenceEditor;
using UnityEngine;
using UnityEngine.Audio;



public class AnimationData
{
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

    [HideInInspector] public List<AnimationClipData> AnimationClipDataList;

    public void SetDataClipList(List<AnimationClipData> dataClipList)
    {
        AnimationClipDataList = dataClipList;
    }

    public string[] GetAnimationNames()
    {
        List<string> strings = new() { "None" };

        for (int i = 0; i < AnimationClipDataList.Count; i++)
        {
            string name = $"{i}_{AnimationClipDataList[i].Clip.name}";

            strings.Add(name);
        }

        return strings.ToArray();
    }

    public AnimationWrapper GetAnimationWrapper()
    {
        if (AnimationClipDataList.Count == 0) return null;

        if (Animation.name == AttackType.None.ToString()) return null;

        char split = '_';
        string[] stringSplit = Animation.name.Split(split);

        if (stringSplit[0] == "") return null;

        float distance = AnimationClipDataList[int.Parse(stringSplit[0])].DistanceOffset;
        float moveTime = 0f;

        GameObject followTimeline = null;
        GameObject attackTimeline = null;

        if (OverrideDistanceOffset)
            distance = DistanceOffset;

        if (OverrideMoveTime)
            moveTime = MoveTime;

        if (OverrideCamera)
        {
            followTimeline = FollowTimeline;
            attackTimeline = AttackTimeline;
        }


        return new AnimationWrapper(stringSplit[1], distance, followTimeline, attackTimeline, AudioResource, moveTime);
    }



    public bool OverrideDistance()
    {
        if (!OverrideDistanceOffset) return false;

        if (Animation.name == AttackType.None.ToString()) return false;

        return true;
    }

    public bool OverrideMove()
    {
        if (!OverrideMoveTime) return false;

        if (Animation.name == AttackType.None.ToString()) return false;

        return true;
    }

    public bool OverrideVirtualCamera()
    {
        if (!OverrideCamera) return false;

        return true;
    }

    
}
