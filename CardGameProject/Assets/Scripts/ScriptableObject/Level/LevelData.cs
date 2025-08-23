using System.Collections.Generic;
using UnityEngine;
using MyBox;
using SceneReference = Eflatun.SceneReference.SceneReference;

public class LevelData : ScriptableObject
{
    [ReadOnly] public string Guid;

    public string LevelName;
    [HideInInspector] public GameObject Prefab;
    public SceneReference SceneRef;
    public AudioData Music;

    [Separator]

    public bool IsFixed;


    [ConditionalField(nameof(IsFixed), false, true)] public int RecommendLevel = 1;
    [ConditionalField(nameof(IsFixed), false, true)] public CollectionWrapper<EnemyData> ListOfEnemies;
    [ConditionalField(nameof(IsFixed), false, true)] [OverrideLabel("Gear Reward")] public CollectionWrapper<WeaponData> ListOfGear;
}
