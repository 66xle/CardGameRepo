using System;
using MyBox;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using SerializeReferenceEditor;

[Serializable]
[SRName("Card")]
public class WeaponCardData 
{
    public Card Card;
    
    [HideInInspector] public List<AnimationClip> AnimationClipList;

    [DefinedValues(nameof(GetAnimationList))] public string Animation;


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

    
}
