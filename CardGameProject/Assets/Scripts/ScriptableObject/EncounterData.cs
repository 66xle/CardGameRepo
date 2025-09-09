using System.Collections.Generic;
using UnityEngine;
using MyBox;

public class EncounterData : ScriptableObject
{
    [ReadOnly] public string Guid;

    public string EncounterName;

    [Separator]

    public List<EnemyData> Elites;
    public List<EnemyData> Minions;

    public EncounterData() { }

    public EncounterData(EncounterData data)
    {
        EncounterName = data.EncounterName;
        Elites = data.Elites;
        Minions = data.Minions;
    }

    // reinforcement setting



}
