using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Loot Table", menuName = "Loot Table")]
public class LootTable : ScriptableObject
{
    public List<WeaponData> CommonGear;
    public List<WeaponData> RareGear;
    public List<WeaponData> EpicGear;
    public List<WeaponData> LegendaryGear;
}
