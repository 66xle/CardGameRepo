using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DifficultyManager : MonoBehaviour
{
    [Header("Difficulty Progression")]
    public int currentScore = 0;
    public int scoreIncrement = 5;

    [Header("Randomization Settings")]
    [Tooltip("How close to the target score a composition must be.")]
    public int acceptableRange = 3;

    [Header("References")]
    public StatsManager statsManager;

    [Header("Enemy Data")]
    public List<EnemyData> minionListData;
    public List<EnemyData> eliteListData;

    const int maxEnemiesPerWave = 3;
    const int minionCost = 1;
    const int eliteCost = 6;

    private void Awake()
    {
        if (GameManager.Instance.DifficultyScore > 0)
        {
            currentScore = GameManager.Instance.DifficultyScore;
        }

        if (minionListData.Count == 0)
        {
            Debug.LogAssertion("Difficulty Manager: No Minon Data in List.");
        }

        if (eliteListData.Count == 0)
        {
            Debug.LogAssertion("Difficulty Manager: No Elite Data in List.");
        }
    }

    public void OnBattleComplete()
    {
        currentScore += scoreIncrement;

        GameManager.Instance.DifficultyScore = currentScore;
    }

    public List<EnemyData> GetEnemies()
    {
        int baseScore = statsManager.Level * 2;
        int targetScore = Mathf.Max(currentScore - baseScore, 0);

        List<EnemyData> composition;

        if (currentScore < 10)
        {
            float bias = Mathf.Clamp01(targetScore / 10f);
            int minionsToSpawn = Mathf.Clamp(
                Mathf.RoundToInt(Random.Range(1, maxEnemiesPerWave + bias)),
                1,
                maxEnemiesPerWave
            );
            composition = new List<EnemyData>();
            for (int i = 0; i < minionsToSpawn; i++)
                composition.Add(minionListData[Random.Range(0, minionListData.Count)]);
        }
        else
        {
            composition = GenerateRandomizedCloseComposition(targetScore);
        }

        Debug.Log($"[Difficulty] Wave Score: {currentScore} | Player Level: {statsManager.Level} | Target Score: {targetScore} | Enemies: {string.Join(", ", composition)}");

        foreach (EnemyData enemyData in composition)
        {
            enemyData.Level = statsManager.Level;
        }

        return composition;
    }

    List<EnemyData> GenerateRandomizedCloseComposition(int targetScore)
    {
        const int maxAttempts = 20;
        System.Random rng = new System.Random();

        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            int enemyCount = rng.Next(1, maxEnemiesPerWave + 1);
            int eliteCount = rng.Next(0, enemyCount + 1);
            int minionCount = enemyCount - eliteCount;

            int compositionScore = (eliteCount * eliteCost) + (minionCount * minionCost);

            Debug.Log($"Composition Score: {compositionScore}");

            if (Mathf.Abs(compositionScore - targetScore) <= acceptableRange)
            {
                List<EnemyData> result = new List<EnemyData>();
                result.AddRange(CreateList(eliteListData[Random.Range(0, eliteListData.Count)], eliteCount));
                result.AddRange(CreateList(minionListData[Random.Range(0, minionListData.Count)], minionCount));
                return result;
            }
        }

        // Fallback: just spawn 1 Minion
        return new List<EnemyData> { minionListData[Random.Range(0, minionListData.Count)] };
    }

    List<EnemyData> CreateList(EnemyData data, int count)
    {
        List<EnemyData> list = new List<EnemyData>();
        for (int i = 0; i < count; i++)
            list.Add(data);
        return list;
    }

}
