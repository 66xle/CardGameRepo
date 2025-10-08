using System.Collections.Generic;
using UnityEngine;
using SerializeReferenceEditor;
using UnityEngine.Playables;
using System;
using MyBox;



[SRName("Weapon")]
public class WeaponAnimationData : AnimationData
{
    [HideInInspector] public List<AnimationClipData> AnimationClipDataList;

    public override string Animation => _animation;
    public override Boolean OverrideDistanceOffset => _overrideDistanceOffset;
    public override float DistanceOffset => _distanceOffset;
    public override Boolean OverrideMoveTime => _overrideMoveTime;
    public override float MoveTime => _moveTime;
    public override Boolean OverrideCamera => _overrideCamera;
    public override PlayableAsset FollowTimeline => _followTimeline;
    public override PlayableAsset AttackTimeline => _attackTimeline;


    [DefinedValues(nameof(GetAnimationNames))] public string _animation = AttackType.None.ToString();
    [ConditionalField(nameof(_animation), true, AttackType.None)] public AudioType AudioType;

    [ConditionalField(nameof(_animation), true, AttackType.None)] public Boolean _overrideDistanceOffset = Boolean.False;
    [ConditionalField(false, nameof(OverrideDistance))] public float _distanceOffset = 0;

    [ConditionalField(nameof(_animation), true, AttackType.None)] public Boolean _overrideMoveTime = Boolean.False;
    [ConditionalField(false, nameof(OverrideMove))] public float _moveTime = 0;

    [ConditionalField(nameof(_animation), true, AttackType.None)] public Boolean _overrideCamera = Boolean.False;
    [ConditionalField(false, nameof(OverrideVirtualCamera))] public PlayableAsset _followTimeline;
    [ConditionalField(false, nameof(OverrideVirtualCamera))] public PlayableAsset _attackTimeline;


    public override void SetDataClipList(List<AnimationClipData> dataClipList)
    {
        AnimationClipDataList = dataClipList;
    }

    public override string[] GetAnimationNames()
    {
        List<string> strings = new() { "None" };

        for (int i = 0; i < AnimationClipDataList.Count; i++)
        {
            string name = $"{i}_{AnimationClipDataList[i].Clip.name}";

            strings.Add(name);
        }

        return strings.ToArray();
    }

    public override AnimationWrapper GetAnimationWrapper()
    {
        if (AnimationClipDataList.Count == 0) return null;

        if (_animation == AttackType.None.ToString()) return null;

        char split = '_';
        string[] stringSplit = _animation.Split(split);

        if (stringSplit[0] == "") return null;

        float distance = AnimationClipDataList[int.Parse(stringSplit[0])].DistanceOffset;
        float moveTime = 0f;

        PlayableAsset followTimeline = null;
        PlayableAsset attackTimeline = null;

        if (OverrideDistanceOffset == Boolean.True)
            distance = DistanceOffset;

        if (OverrideMoveTime == Boolean.True)
            moveTime = MoveTime;

        if (OverrideCamera == Boolean.True)
        {
            followTimeline = FollowTimeline;
            attackTimeline = AttackTimeline;
        }

        
        return new AnimationWrapper(stringSplit[1], distance, followTimeline, attackTimeline, AudioType, moveTime);
    }




}
