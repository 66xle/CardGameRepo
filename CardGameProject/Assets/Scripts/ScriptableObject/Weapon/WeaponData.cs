using MyBox;
using SerializeReferenceEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "WeaponData")]
public class WeaponData : ScriptableObject 
{
    [ReadOnly] public string Guid;

    public string WeaponName;
    public string Description;
    public DamageType DamageType;
    public WeaponType WeaponType;
    private WeaponType _previousWeaponType;

    public GameObject Prefab;

    [HideInInspector] public Transform HolsterSlot;

    [Separator]

    [ReadOnly][SerializeReference][SR] public List<WeaponTypeAnimation> WeaponTypeAnimationSet;
    private int _weaponTypeCount;

    [Separator]

    

    [SerializeReference][SR] public List<WeaponCardData> Cards;


    public void OnValidate()
    {
        if (WeaponType != _previousWeaponType || WeaponTypeAnimationSet.Count != _weaponTypeCount)
        {
            _previousWeaponType = WeaponType;

            FindAllWeaponTypeAnimationData(out List<WeaponTypeAnimationData> data);

            WeaponTypeAnimationSet = new(data.First(data => data.WeaponType == WeaponType).AnimationClipList);
            _weaponTypeCount = WeaponTypeAnimationSet.Count;



            List<AnimationClip> animationClips = new();
            WeaponTypeAnimationSet.ForEach(data => animationClips.AddRange(data.AnimationClipList));

            Cards.ForEach(data => data.AnimationClipList = animationClips);
        }


        if (WeaponTypeAnimationSet == null) return;

        foreach (WeaponCardData data in Cards)
        {
            if (data == null) continue;

            data.UpdateAnimationList(WeaponTypeAnimationSet);
        }
    }

    private void FindAllWeaponTypeAnimationData(out List<WeaponTypeAnimationData> data)
    {
        string[] guids = AssetDatabase.FindAssets("t:WeaponTypeAnimationData");

        data = new List<WeaponTypeAnimationData>();

        for (int i = 0; i < guids.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[i]);

            WeaponTypeAnimationData loadedData = AssetDatabase.LoadAssetAtPath<WeaponTypeAnimationData>(path);

            data.Add(loadedData);
        }
    }
}
