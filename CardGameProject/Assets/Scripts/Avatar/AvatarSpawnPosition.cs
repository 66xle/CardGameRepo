using System;
using System.Collections.Generic;
using MyBox;
using UnityEngine;

[Serializable]
public struct SpawnPosition
{
    public string Name;
    public Transform Transform;
    public GameObject Prefab;
}

public class AvatarSpawnPosition : MonoBehaviour
{
    public List<Transform> EnemySpawnPositionList;
    public Transform PlayerSpawnPosition;
    public Transform KnightSpawnPosition;
    public Transform CutsceneParent;
    [MustBeAssigned] public EndlessTunnelManagerUnified Endless;

    [Separator]

    public List<SpawnPosition> CutsceneSpawnPositions;
    

    public void Awake()
    {
        ServiceLocator.Register(this);
    }

    public void OnDestroy()
    {
        ServiceLocator.Unregister<AvatarSpawnPosition>();
    }
}
