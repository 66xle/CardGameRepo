using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Loot Table", menuName = "Loot Table")]
public class LootTable : ScriptableObject
{
    public List<GearData> CommonGear;
    public List<GearData> RareGear;
    public List<GearData> EpicGear;
    public List<GearData> LegendaryGear;
}
