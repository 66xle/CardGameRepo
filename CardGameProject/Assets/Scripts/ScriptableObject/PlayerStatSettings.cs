using UnityEngine;

[CreateAssetMenu(fileName = "Player Stats Settings", menuName = "Player Stats Settings")]
public class PlayerStatSettings : ScriptableObject
{
    [SerializeField] Vector3 Experience;
    [SerializeField] Vector3 Health;
    [SerializeField] Vector3 Stamina;
    [SerializeField] Vector3 Defence;
    [SerializeField] int DefencePercentage;

    public float CalculateHealth(float level)
    {
        Vector3 stat = Health;

        float a = stat.x * level;
        float b = stat.y * level;
        float c = stat.z;

        return Mathf.Pow(a, 2) + b + c;
    }

    public float CalculateStamina(float level)
    {
        Vector3 stat = Stamina;

        float a = stat.x * level;
        float b = stat.y * level;
        float c = stat.z;

        return Mathf.Pow(a, 2) + b + c;
    }

    public float CalculateDefence(float level)
    {
        Vector3 stat = Defence;

        float a = stat.x * level;
        float b = stat.y * level;
        float c = stat.z;

        float def = Mathf.Pow(a, 2) + b + c;

        Debug.Log(def / (def + DefencePercentage));

        return def / (def + DefencePercentage);
    }


}
