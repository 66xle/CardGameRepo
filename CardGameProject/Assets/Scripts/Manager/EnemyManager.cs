using System;
using System.Collections.Generic;
using MyBox;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyManager : MonoBehaviour
{
    [MinMaxRange(1, 3)] [SerializeField] RangedInt AmountOfEnemies = new RangedInt(1, 3);
    [ReadOnly] public List<Transform> EnemySpawnPosList;
    public List<GameObject> EnemyUISpawnPosList;

    [Header("References")]
    [MustBeAssigned] [SerializeField] CombatUIManager CombatUIManager;
    [MustBeAssigned] [SerializeField] CombatStateMachine Ctx;
    [MustBeAssigned] [SerializeField] EnemyStatSettings ESS;
    [MustBeAssigned] [SerializeField] DifficultyManager DifficultyManager;

    private void Awake()
    {
        if (EnemySpawnPosList.Count == 0)
        {
            Debug.LogAssertion("Enemy Manager: No spawn position in List.");
        }

        if (EnemyUISpawnPosList.Count == 0)
        {
            Debug.LogAssertion("Enemy Manager: No UI spawn pos in List.");
        }
    }

    public List<EnemyData> GetEnemies()
    {
        return DifficultyManager.GetEnemies();

        List<EnemyData> getEnemies = new();
        List<EnemyData> enemies = DifficultyManager.GetEnemies();

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

    public List<Enemy> InitEnemies(List<EnemyData> enemyDataList)
    {
        List<Enemy> enemies = new();

        // Spawn Enemy
        for (int i = 0; i < enemyDataList.Count; i++)
        {
            // Init Obj
            Enemy enemy = Instantiate(enemyDataList[i].Prefab, EnemySpawnPosList[i]).GetComponent<Enemy>();
            enemy.InitStats(enemyDataList[i], ESS);
            enemies.Add(enemy);

            GameObject statsUI = Instantiate(CombatUIManager.enemyUIPrefab, EnemyUISpawnPosList[i].GetComponent<RectTransform>());
            enemy.InitUI(statsUI, CombatUIManager.detailedUI);

            EnemyUI enemyUI = statsUI.GetComponent<EnemyUI>();
            enemyUI.Init(Ctx, enemy);
        }

        return enemies;
    }

}
