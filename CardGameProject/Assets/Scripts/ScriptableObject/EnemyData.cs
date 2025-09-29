using System.Collections.Generic;
using SerializeReferenceEditor;
using UnityEngine;
using MyBox;
using System.Linq;
using UnityEditor;
using PixelCrushers.DialogueSystem;
using UnityEngine.Rendering.Universal;

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
    public AudioData HurtSounds;
    public AudioData DeathSounds;


    [Separator]

    public EnemyType EnemyType;
    [ConditionalField(nameof(EnemyType), false, EnemyType.Elite)] public bool HasDialogue;
    [ConditionalField(nameof(HasDialogue), false, true)] public DialogueDatabase DialogueDatabase;
    [ConditionalField(false, nameof(HasDialogueDatabase))] public string ConversationTitle;

    [Separator]

    public WeaponType WeaponType;
    [ReadOnly][SerializeReference][SR] public List<WeaponTypeAnimation> WeaponTypeAnimationSet;

    [SerializeReference][SR] public List<CardAnimationData> Cards;

    private List<AnimationClipData> AnimationClipDataList;
    private WeaponType _previousWeaponType;
    private int _weaponTypeCount;

    public bool HasDialogueDatabase()
    {
        if (DialogueDatabase == null || !HasDialogue)
            return false;

        return true;
    }

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

            AnimationClipDataList = animationClipDataList;

        }

        if (WeaponTypeAnimationSet == null) return;

        foreach (CardAnimationData data in Cards)
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
