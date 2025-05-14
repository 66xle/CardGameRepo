using System;
using MyBox;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using SerializeReferenceEditor;
using System.Linq;

[Serializable]
[SRName("Card")]
public class WeaponCardData 
{
    public Card Card;
    
    [HideInInspector] public List<AnimationClip> AnimationClipList;
    [DefinedValues(nameof(GetAnimationList))] public string Animation;
    [ReadOnly] public List<string> AnimationList = new();

    private string[] GetAnimationList()
    {
        List<string> strings = new() { "Strike", "Heavy", "AOE" };

        for (int i = 0; i < AnimationClipList.Count; i++)
        {
            string name = AnimationClipList[i].name;

            strings.Add(name);
        }

        return strings.ToArray();
    }


    public void UpdateAnimationList(List<WeaponTypeAnimation> WeaponTypeAnimationSet)
    {
        AnimationList.Clear();

        int index = -1;

        if (Animation == "Strike")
            index = 0;
        else if (Animation == "Heavy")
            index = 1;
        else if (Animation == "AOE")
            index = 2;


        if (index == -1)
        {
            AnimationList.Add(Animation);
            return;
        }

        WeaponTypeAnimationSet[index].AnimationClipList.ForEach(clip => AnimationList.Add(clip.name));
    }
}
