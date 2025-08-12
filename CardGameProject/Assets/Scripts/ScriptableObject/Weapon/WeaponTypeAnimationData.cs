using System.Collections.Generic;
using UnityEngine;
using SerializeReferenceEditor;
using UnityEditor;
using MyBox;

#if UNITY_EDITOR

[CreateAssetMenu(fileName = "New Weapon Type Animation Data", menuName = "Weapon Type Animation")] 
public class WeaponTypeAnimationData : ScriptableObject
{
    public WeaponType WeaponType;

    [SerializeReference][SR] public List<WeaponTypeAnimation> AnimationClipList;

    [ButtonMethod]
    public void UpdateAllWeaponTypeAnimation()
    {
        FindAllEnemyData(out List<EnemyData> enemyData);

        FindAllWeaponData(out List<WeaponData> weaponData);

        enemyData.ForEach(data =>
        {
            if (data.WeaponType == WeaponType)
            {
                data.WeaponTypeAnimationSet = AnimationClipList;
            }
        });

        weaponData.ForEach(data =>
        {
            if (data.WeaponType == WeaponType)
            {
                data.WeaponTypeAnimationSet = AnimationClipList;
            }
        });

        Debug.Log($"Updated Weapon Type Animation Set: {WeaponType}");
    }

    private void FindAllEnemyData(out List<EnemyData> data)
    {
        string[] guids = AssetDatabase.FindAssets("t:EnemyData");

        data = new List<EnemyData>();

        for (int i = 0; i < guids.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[i]);

            EnemyData loadedData = AssetDatabase.LoadAssetAtPath<EnemyData>(path);

            data.Add(loadedData);
        }
    }

    private void FindAllWeaponData(out List<WeaponData> data)
    {
        string[] guids = AssetDatabase.FindAssets("t:WeaponData");

        data = new List<WeaponData>();

        for (int i = 0; i < guids.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[i]);

            WeaponData loadedData = AssetDatabase.LoadAssetAtPath<WeaponData>(path);

            data.Add(loadedData);
        }
    }
}

#endif
