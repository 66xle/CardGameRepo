using System.Collections.Generic;
using SerializeReferenceEditor;
using UnityEngine;
using MyBox;
using static UnityEngine.Rendering.GPUSort;
using System.Linq;
using UnityEditor;


public enum EnemyType
{
    Minion,
    Elite,
    Boss
}

[CreateAssetMenu(fileName = "New Enemy", menuName = "Enemy")]
public class EnemyData : ScriptableObject
{
    [ReadOnly] public string Guid;
    public GameObject Prefab;
    public string Name;
    public int Guard;
    public int Level = 1;

    public EnemyType EnemyType;
    public WeaponType WeaponType;
    [ReadOnly][SerializeReference][SR] public List<WeaponTypeAnimation> WeaponTypeAnimationSet;

    [SerializeReference][SR] public List<WeaponCardData> Cards;

    private WeaponType _previousWeaponType;
    private int _weaponTypeCount;


#if UNITY_EDITOR
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
#endif
}
