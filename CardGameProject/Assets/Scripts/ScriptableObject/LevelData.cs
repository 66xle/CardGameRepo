using System.Collections.Generic;
using UnityEngine;
using MyBox;

public class LevelData : ScriptableObject
{
    [ReadOnly] public string Guid;

    public string LevelName;
    public GameObject Prefab;

    public bool IsFixed;
}
