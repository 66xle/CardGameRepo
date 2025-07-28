using System.Collections.Generic;
using System.Linq;
using UnityEngine;


// Helper class to store composition info
struct Combination
{
    public int minions;
    public int elites;
    public int score;
}

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
        int baseScore = GameManager.Instance.PlayerLevel * 2;
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

            int compositionScore = minionsToSpawn * minionCost;
            Debug.Log($"Composition Score: {compositionScore}");
        }
        else
        {
            composition = GenerateRandomizedCloseComposition(targetScore);
        }

        Debug.Log($"[Difficulty] Wave Score: {currentScore} | Player Level: {GameManager.Instance.PlayerLevel} | Target Score: {targetScore} | Enemies: {string.Join(", ", composition)}");

        foreach (EnemyData enemyData in composition)
        {
            enemyData.Level = GameManager.Instance.PlayerLevel;
        }

        return composition;
    }

    List<EnemyData> GenerateRandomizedCloseComposition(int targetScore)
    {
        // Build all possible combinations (1–3 enemies)
        List<Combination> allCombinations = new List<Combination>();

        for (int enemyCount = 1; enemyCount <= maxEnemiesPerWave; enemyCount++)
        {
            for (int eliteCount = 0; eliteCount <= enemyCount; eliteCount++)
            {
                int minionCount = enemyCount - eliteCount;
                int compositionScore = (eliteCount * eliteCost) + (minionCount * minionCost);

                allCombinations.Add(new Combination
                {
                    minions = minionCount,
                    elites = eliteCount,
                    score = compositionScore
                });
            }
        }

        int currentRange = acceptableRange;
        List<Combination> validCombinations = new List<Combination>();

        // Keep expanding range until we find something
        while (validCombinations.Count == 0)
        {
            validCombinations = allCombinations.FindAll(c => Mathf.Abs(c.score - targetScore) <= currentRange);

            if (validCombinations.Count == 0)
            {
                currentRange++;
            }
        }

        // Pick a random combination
        Combination selected = validCombinations[Random.Range(0, validCombinations.Count)];
        Debug.Log($"Combination Score: {selected.score}");

        // Build the enemy list
        List<EnemyData> result = new List<EnemyData>();
        result.AddRange(CreateList(eliteListData[Random.Range(0, eliteListData.Count)], selected.elites));
        result.AddRange(CreateList(minionListData[Random.Range(0, minionListData.Count)], selected.minions));

        return result;
    }

    List<EnemyData> CreateList(EnemyData data, int count)
    {
        List<EnemyData> list = new List<EnemyData>();
        for (int i = 0; i < count; i++)
            list.Add(data);
        return list;
    }

}
