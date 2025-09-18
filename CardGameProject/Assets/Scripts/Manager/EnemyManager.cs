using System;
using System.Collections.Generic;
using MyBox;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyManager : MonoBehaviour
{
    [ReadOnly] public List<Transform> EnemySpawnPosList;
    public List<GameObject> EnemyUISpawnPosList;

    [Header("References")]
    [MustBeAssigned] [SerializeField] CombatUIManager CombatUIManager;
    [MustBeAssigned] [SerializeField] CombatStateMachine Ctx;
    [MustBeAssigned] [SerializeField] EnemyStatSettings ESS;
    [MustBeAssigned] [SerializeField] DifficultyManager DifficultyManager;
    [MustBeAssigned] [SerializeField] CardManager CardManager;
    [MustBeAssigned] [SerializeField] LevelManager LevelManager; // Editor only

    private List<GameObject> spawnedObjects = new();

    private void Awake()
    {
        SceneInitialize.Instance.Subscribe(Init);
    }

    private void Init()
    {
#if UNITY_EDITOR
        if (!LevelManager.isEnvironmentLoaded) return;
#endif

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
            enemyUI.Init(Ctx, enemy, this);

            spawnedObjects.Add(enemy.gameObject);
            spawnedObjects.Add(statsUI);
        }

        return enemies;
    }

    public void SelectEnemy(Enemy enemy)
    {
        CardManager.UpdateCardsInHand(enemy);
        enemy.EnemySelection(true);
    }

    public void ClearEnemiesAndUI()
    {
        foreach (GameObject spawnedObject in spawnedObjects)
        {
            Destroy(spawnedObject);
        }

        spawnedObjects.Clear();
    }
}
