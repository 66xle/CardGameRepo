using System;
using MyBox;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using SerializeReferenceEditor;
using System.Linq;
using UnityEngine.Rendering;
using UnityEngine.Playables;

public enum Boolean
{
    True,
    False
}


public enum AttackType
{
    None,
    Strike,
    Heavy,
    AOE
}

[Serializable]
[SRName("Card")]
public class WeaponCardData 
{
    public Card Card;
    public int CardAmount = 1;
    
    [HideInInspector] public List<AnimationClipData> AnimationClipDataList;
    [DefinedValues(nameof(GetAnimationList))] public string Animation;
    [ConditionalField(nameof(Animation), true, AttackType.None, AttackType.Strike, AttackType.Heavy, AttackType.AOE)] public Boolean OverrideDistanceOffset = Boolean.False;
    [ConditionalField(false, nameof(OverrideDistance))] public float DistanceOffset = 0;
    [ConditionalField(nameof(Animation), true, AttackType.None, AttackType.Strike, AttackType.Heavy, AttackType.AOE)] public Boolean OverrideCamera = Boolean.False;
    [ConditionalField(false, nameof(OverrideVirtualCamera))] public PlayableAsset FollowTimeline;
    [ConditionalField(false, nameof(OverrideVirtualCamera))] public PlayableAsset AttackTimeline;

    [HideInInspector] public List<AnimationWrapper> AnimationList = new();

    private string[] GetAnimationList()
    {
        List<string> strings = new() { "None", "Strike", "Heavy", "AOE" };

        for (int i = 0; i < AnimationClipDataList.Count; i++)
        {
            string name = $"{i}_{AnimationClipDataList[i].Clip.name}";

            strings.Add(name);
        }

        return strings.ToArray();
    }


    public void UpdateAnimationList(List<WeaponTypeAnimation> WeaponTypeAnimationSet)
    {
        AnimationList.Clear();

        if (Animation == AttackType.None.ToString()) return;

        int index = -1;

        if (Animation == AttackType.Strike.ToString())
            index = 0;
        else if (Animation == AttackType.Heavy.ToString())
            index = 1;
        else if (Animation == AttackType.AOE.ToString())
            index = 2;

        // Single Animation
        if (index == -1)
        {
            char split = '_';
            string[] stringSplit = Animation.Split(split);
            float distance = AnimationClipDataList[int.Parse(stringSplit[0])].DistanceOffset;

            PlayableAsset followTimeline = null;
            PlayableAsset attackTimeline = null;

            if (OverrideDistanceOffset == Boolean.True)
                distance = DistanceOffset;

            if (OverrideCamera == Boolean.True)
            {
                followTimeline = FollowTimeline;
                attackTimeline = AttackTimeline;
            }

            AnimationList.Add(new AnimationWrapper(stringSplit[1], distance, followTimeline, attackTimeline));
            return;
        }

        // Weapon Type Category
        WeaponTypeAnimationSet[index].AnimationClipDataList.ForEach(clipData => AnimationList.Add(new AnimationWrapper(clipData)));
    }

    public bool OverrideDistance()
    {
        if (OverrideDistanceOffset == Boolean.False) return false;

        if (Animation == AttackType.None.ToString()) return false;

        if (Animation == AttackType.Strike.ToString())
            return false;
        else if (Animation == AttackType.Heavy.ToString())
            return false;
        else if (Animation == AttackType.AOE.ToString())
            return false;

        return true;
    }


    public bool OverrideVirtualCamera()
    {
        if (OverrideCamera == Boolean.False) 
            return false;

        return true;
    }
}
