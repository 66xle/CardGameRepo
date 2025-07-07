using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Level Setting", menuName = "Level Setting")]
public class LevelSettings : ScriptableObject
{
    public List<LevelData> Levels;
}
