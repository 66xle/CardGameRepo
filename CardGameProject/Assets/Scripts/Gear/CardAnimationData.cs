using MyBox;
using NUnit.Framework;
using SerializeReferenceEditor;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;


[Serializable]
[SRName("Card")]
public class CardAnimationData
{
    public Card Card;
    public int CardAmount = 1;

    [SerializeReference][SR] public List<AnimationData> Animations;

    [HideInInspector] public List<AnimationWrapper> AnimationList = new();


    public void UpdateClipData(List<AnimationClipData> animationClipDataList)
    {
        Debug.Log("update");

        foreach (AnimationData animationData in Animations)
        {
            if (animationData == null) continue;

            animationData.SetDataClipList(animationClipDataList);
        }
    }

    public void UpdateAnimationList()
    {
        AnimationList.Clear();

        foreach (AnimationData animationData in Animations)
        {
            if (animationData == null) continue;

            AnimationWrapper wrapper = animationData.GetAnimationWrapper();

            if (wrapper == null) continue;

            AnimationList.Add(wrapper);
        }
    }

}
