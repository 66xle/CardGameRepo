using System;
using System.Collections.Generic;
using NUnit.Framework;
using SerializeReferenceEditor;
using UnityEngine;


[Serializable]
[SRHidden]
public class WeaponTypeAnimation
{
    public List<AnimationClip> AnimationClipList;
}
