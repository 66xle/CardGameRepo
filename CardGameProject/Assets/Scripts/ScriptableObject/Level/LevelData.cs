using System.Collections.Generic;
using UnityEngine;
using MyBox;
using SceneReference = Eflatun.SceneReference.SceneReference;
using SerializeReferenceEditor;
using System.Linq;
using UnityEngine.Audio;

public class LevelData : ScriptableObject
{
    [ReadOnly] public string Guid;

    public string LevelName;
    public GameObject Prefab;
    //public SceneReference SceneRef;
    public AudioData Music;

    [Separator]

    public bool IsFixed;

    [ConditionalField(nameof(IsFixed), false, true)] public int RecommendLevel = 1;
    [ConditionalField(nameof(IsFixed), false, true)] public CollectionWrapper<CollectionWrapper<EnemyData>> CollectionEnemies;
    [ConditionalField(nameof(IsFixed), false, true)] public CollectionWrapper<CollectionWrapper<GearData>> CollectionGear;
    [ConditionalField(nameof(IsFixed), false, true)] public CollectionWrapper<AudioResource> CollectionMusic;


    public List<EnemyData> GetEnemyList(int waveIndex)
    {
        CollectionWrapper<EnemyData> collectionEnemy = CollectionEnemies.Value[waveIndex];
        return collectionEnemy.Value.ToList();
    }

    public bool IsWaveLimitReached(int currentWave)
    {
        if (CollectionEnemies.Value.Length == currentWave) return true;

        return false;
    }

    public AudioResource GetMusic(int index)
    {
        AudioResource resource = CollectionMusic.Value[index];
        return resource;
    }
}
