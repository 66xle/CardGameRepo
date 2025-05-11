using System.Collections.Generic;
using UnityEngine;
using SerializeReferenceEditor;

[CreateAssetMenu(fileName = "New Weapon Type Animation Data", menuName = "Weapon Type Animation")] 
public class WeaponTypeAnimationData : ScriptableObject
{
    public WeaponType WeaponType;

    [SerializeReference][SR] public List<WeaponTypeAnimation> AnimationClipList;
}
