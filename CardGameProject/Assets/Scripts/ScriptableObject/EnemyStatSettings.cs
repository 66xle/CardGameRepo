using UnityEngine;

[CreateAssetMenu(fileName = "Enemy Stats Settings", menuName = "Enemy Stats Settings")]
public class EnemyStatSettings : ScriptableObject
{
    [Header("Formula")]
    [SerializeField] Vector3 Damage;
    [SerializeField] Vector3 Health;
    [SerializeField] Vector3 Defence;
    [SerializeField] int DefencePercentage;

    [Header("Multiplier")]
    [SerializeField] Vector3 DamageMultiplier;
    [SerializeField] Vector3 HealthMultiplier;
    [SerializeField] Vector3 DefenceMultiplier;

    public float CalculateHealth(float level, EnemyType type)
    {
        Vector3 stat = Health;

        float a = stat.x * level;
        float b = stat.y * level;
        float c = stat.z;

        float value = Mathf.Pow(a, 2) + b + c;

        return value * GetMultiplier(HealthMultiplier, type);
    }

    public float CalculateDamage(float level, EnemyType type)
    {
        Vector3 stat = Damage;

        float a = stat.x * level;
        float b = stat.y * level;
        float c = stat.z;

        float value = Mathf.Pow(a, 2) + b + c;

        return value * GetMultiplier(DamageMultiplier, type);
    }

    public float CalculateDefence(float level, EnemyType type)
    {
        Vector3 stat = Defence;

        float a = stat.x * level;
        float b = stat.y * level;
        float c = stat.z;

        float def = Mathf.Pow(a, 2) + b + c;

        float value = def * GetMultiplier(DamageMultiplier, type);

        return value / (value + DefencePercentage);
    }

    float GetMultiplier(Vector3 multiplier, EnemyType type)
    {
        if (type == EnemyType.Elite) return multiplier.y;
        if (type == EnemyType.Boss) return multiplier.z;

        return multiplier.x;
    }
}