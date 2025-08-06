using UnityEngine;

[CreateAssetMenu(fileName = "Player Stats Settings", menuName = "Player Stats Settings")]
public class PlayerStatSettings : ScriptableObject
{
    [SerializeField] Vector3 Experience;
    [SerializeField] Vector3 Health;
    [SerializeField] Vector3 Stamina;
    [SerializeField] Vector3 Attack;
    [SerializeField] Vector3 Defence;
    [SerializeField] int DefencePercentage;

    public float CalculateExperience(float level)
    {
        Vector3 stat = Experience;

        float a = stat.x * level;
        float b = stat.y * level;
        float c = stat.z;

        return Mathf.RoundToInt(Mathf.Pow(a, 2) + b + c);
    }

    public float CalculateHealth(float level)
    {
        Vector3 stat = Health;

        float a = stat.x * level;
        float b = stat.y * level;
        float c = stat.z;

        return Mathf.RoundToInt(Mathf.Pow(a, 2) + b + c);
    }

    public float CalculateStamina(float level)
    {
        Vector3 stat = Stamina;

        float a = stat.x * level;
        float b = stat.y * level;
        float c = stat.z;

        return Mathf.RoundToInt(Mathf.Pow(a, 2) + b + c);
    }

    public float CalculateDefence(float level)
    {
        Vector3 stat = Defence;

        float a = stat.x * level;
        float b = stat.y * level;
        float c = stat.z;

        float def = Mathf.RoundToInt(Mathf.Pow(a, 2) + b + c);

        return def;
    }

    public float GetDefencePercentage()
    {
        return DefencePercentage;
    }

    public float CalculateAttack(float level)
    {
        Vector3 stat = Attack;

        float a = stat.x * level;
        float b = stat.y * level;
        float c = stat.z;

        return Mathf.RoundToInt(Mathf.Pow(a, 2) + b + c);
    }
}
