using MyBox;
using SerializeReferenceEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Analytics;

public enum Rarity
{
    Common,
    Rare,
    Epic,
    Legendary
}

public class WeaponData : GearData 
{
    public DamageType DamageType;
    public WeaponType WeaponType;

    public override int Value => WeaponAttack;
    [ReadOnly] public int WeaponAttack;

    // Equipment, Switch Weapon
    [HideInInspector] public Transform HolsterSlot; 

    [Separator]

    // Editor
    [ReadOnly][SerializeReference][SR] public List<WeaponTypeAnimation> WeaponTypeAnimationSet;

    [ReadOnly] public List<AnimationClipData> AnimationClipDataList;
    private WeaponType _previousWeaponType;
    private int _weaponTypeCount;

    [Separator]

    public override List<CardAnimationData> Cards => _cards; 
    [SerializeReference][SR] public List<CardAnimationData> _cards;

    public WeaponData() { }

    public WeaponData(WeaponData data)
    {
        GearName = data.GearName;
        DamageType = data.DamageType;
        WeaponType = data.WeaponType;
        Description = data.Description;
        Rarity = data.Rarity;
        _cards = data._cards;
        Prefab = data.Prefab;
    }


#if UNITY_EDITOR
    public void OnValidate()
    {
        if (WeaponType != _previousWeaponType || WeaponTypeAnimationSet.Count != _weaponTypeCount)
        {
            _previousWeaponType = WeaponType;

            FindAllWeaponTypeAnimationData(out List<WeaponTypeAnimationData> data);

            // Check weapon type
            WeaponTypeAnimationSet = new(data.First(data => data.WeaponType == WeaponType).AnimationClipList);
            _weaponTypeCount = WeaponTypeAnimationSet.Count;

            // Add to list
            List<AnimationClipData> animationClipDataList = new();
            WeaponTypeAnimationSet.ForEach(data => animationClipDataList.AddRange(data.AnimationClipDataList));

            AnimationClipDataList = animationClipDataList;

            Debug.Log("clip update");
        }


        if (WeaponTypeAnimationSet == null) return;

        foreach (CardAnimationData data in _cards)
        {
            if (data == null) continue;

            data.UpdateAnimationList();
            data.UpdateClipData(AnimationClipDataList);
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

#endif
}
