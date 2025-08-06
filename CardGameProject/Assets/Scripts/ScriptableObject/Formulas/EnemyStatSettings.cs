using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

[CreateAssetMenu(fileName = "Enemy Stats Settings", menuName = "Enemy Stats Settings")]
public class EnemyStatSettings : ScriptableObject
{
    [Header("Formula")]
    [SerializeField] Vector3 Attack;
    [SerializeField] Vector3 Health;
    [SerializeField] Vector3 Defence;
    [SerializeField] int DefencePercentage;

    [Header("Multiplier")]
    [SerializeField] Vector3 AttackMultiplier;
    [SerializeField] Vector3 HealthMultiplier;
    [SerializeField] Vector3 DefenceMultiplier;

    public float CalculateHealth(float level, EnemyType type)
    {
        Vector3 stat = Health;

        float a = stat.x * level;
        float b = stat.y * level;
        float c = stat.z;

        float value = Mathf.Pow(a, 2) + b + c;

        return Mathf.RoundToInt(value * GetMultiplier(HealthMultiplier, type));
    }

    public float CalculateAttack(float level, EnemyType type)
    {
        Vector3 stat = Attack;

        float a = stat.x * level;
        float b = stat.y * level;
        float c = stat.z;

        float value = Mathf.Pow(a, 2) + b + c;

        return Mathf.RoundToInt(value * GetMultiplier(AttackMultiplier, type));
    }

    public float CalculateDefence(float level, EnemyType type)
    {
        Vector3 stat = Defence;

        float a = stat.x * level;
        float b = stat.y * level;
        float c = stat.z;
        float def = Mathf.Pow(a, 2) + b + c;

        float value = Mathf.RoundToInt(def * GetMultiplier(AttackMultiplier, type));

        return value;
    }

    public float GetDefencePercentage()
    {
        return DefencePercentage;
    }

    float GetMultiplier(Vector3 multiplier, EnemyType type)
    {
        if (type == EnemyType.Elite) return multiplier.y;
        if (type == EnemyType.Boss) return multiplier.z;

        return multiplier.x;
    }
}