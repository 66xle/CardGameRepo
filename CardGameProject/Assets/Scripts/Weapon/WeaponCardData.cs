using System;
using MyBox;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using SerializeReferenceEditor;
using System.Linq;

public enum AttackType
{
    Strike,
    Heavy,
    AOE
}

[Serializable]
[SRName("Card")]
public class WeaponCardData 
{
    public Card Card;
    
    [HideInInspector] public List<AnimationClipData> AnimationClipDataList;
    [DefinedValues(nameof(GetAnimationList))] public string Animation;
    [ReadOnly] public List<string> AnimationList = new();

    private string[] GetAnimationList()
    {
        List<string> strings = new() { "None", "Strike", "Heavy", "AOE" };

        for (int i = 0; i < AnimationClipDataList.Count; i++)
        {
            string name = AnimationClipDataList[i].clip.name;

            strings.Add(name);
        }

        return strings.ToArray();
    }


    public void UpdateAnimationList(List<WeaponTypeAnimation> WeaponTypeAnimationSet)
    {
        AnimationList.Clear();

        int index = -1;

        if (Animation == AttackType.Strike.ToString())
            index = 0;
        else if (Animation == AttackType.Heavy.ToString())
            index = 1;
        else if (Animation == AttackType.AOE.ToString())
            index = 2;


        if (index == -1)
        {
            AnimationList.Add(Animation);
            return;
        }

        WeaponTypeAnimationSet[index].AnimationClipDataList.ForEach(clipData => AnimationList.Add(clipData.clip.name));
    }
}
