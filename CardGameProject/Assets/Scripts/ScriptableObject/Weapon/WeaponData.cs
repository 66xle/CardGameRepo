using MyBox;
using SerializeReferenceEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public enum Rarity
{
    Common,
    Rare,
    Epic,
    Legendary
}

[CreateAssetMenu(menuName = "WeaponData")]
public class WeaponData : ScriptableObject 
{
    [ReadOnly] public string Guid;

    public string WeaponName;
    public string Description;
    public Rarity Rarity;
    public DamageType DamageType;
    public WeaponType WeaponType;
    public GameObject Prefab;
    [HideInInspector] public int WeaponAttack;


    // Reward Manager
    public Texture IconTexture; 

    // Equipment, Switch Weapon
    [HideInInspector] public Transform HolsterSlot; 

    [Separator]

    // Editor
    [ReadOnly][SerializeReference][SR] public List<WeaponTypeAnimation> WeaponTypeAnimationSet; 
    private WeaponType _previousWeaponType;
    private int _weaponTypeCount;

    [Separator]

    [SerializeReference][SR] public List<WeaponCardData> Cards;

    public WeaponData() { }

    public WeaponData(WeaponData data)
    {
        WeaponName = data.WeaponName;
        DamageType = data.DamageType;
        WeaponType = data.WeaponType;
        Description = data.Description;
        Rarity = data.Rarity;
        Cards = data.Cards;
        Prefab = data.Prefab;
    }

    public void OnValidate()
    {
        if (WeaponType != _previousWeaponType || WeaponTypeAnimationSet.Count != _weaponTypeCount)
        {
            _previousWeaponType = WeaponType;

            FindAllWeaponTypeAnimationData(out List<WeaponTypeAnimationData> data);

            WeaponTypeAnimationSet = new(data.First(data => data.WeaponType == WeaponType).AnimationClipList);
            _weaponTypeCount = WeaponTypeAnimationSet.Count;



            List<AnimationClipData> animationClipDataList = new();
            WeaponTypeAnimationSet.ForEach(data => animationClipDataList.AddRange(data.AnimationClipDataList));

            Cards.ForEach(data => data.AnimationClipDataList = animationClipDataList);
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
