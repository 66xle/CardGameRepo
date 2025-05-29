using System;
using System.Collections.Generic;
using MyBox;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] [MinMaxRange(1, 3)] private RangedInt AmountOfEnemies = new RangedInt(1, 3);
    [SerializeField] private List<EnemyData> Enemies = new();

    private void Awake()
    {
        if (Enemies.Count == 0)
        {
            Debug.LogAssertion("Enemy Manager: No Enemy in List.");
        }
    }

    public List<EnemyData> GetEnemies()
    {
        List<EnemyData> getEnemies = new();
        List<EnemyData> enemies = new(Enemies);

        int count = Random.Range(AmountOfEnemies.Min, AmountOfEnemies.Max + 1);

        for (int i = 0; i < count; i++)
        {
            if (enemies.Count == 0)
            {
                Debug.LogAssertion("Enemy Manager: Not enough enemies in List.");
                return getEnemies;
            }
            int index = Random.Range(0, enemies.Count);
            getEnemies.Add(enemies[index]);
            enemies.RemoveAt(index);
        }

        return getEnemies;
    }
}
