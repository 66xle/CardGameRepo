using UnityEngine;



public class StatsManager : MonoBehaviour
{
    [SerializeField] float baseHealth;
    [SerializeField] float baseStamina;
    [SerializeField] int baseGuard;
    public ArmourType armourType;
    public DamageType damageType;

    [HideInInspector] public float currentMaxHealth;
    [HideInInspector] public float currentMaxStamina;
    [HideInInspector] public int currentMaxGuard;

    [Header("Armour Stamina")]
    [Range(0, 2)] public float lightMultiplier;
    [Range(0, 2)] public float mediumMultiplier;
    [Range(0, 2)] public float heavyMultiplier;


    void Start() // Initalise variables for now
    {
        currentMaxHealth = baseHealth;
        currentMaxGuard = baseGuard;

        if (armourType == ArmourType.Light)
        {
            currentMaxStamina = Mathf.RoundToInt(baseStamina * lightMultiplier);
        }
        else if (armourType == ArmourType.Medium)
        {
            currentMaxStamina = Mathf.RoundToInt(baseStamina * mediumMultiplier);
        }
        else if (armourType == ArmourType.Heavy)
        {
            currentMaxStamina = Mathf.RoundToInt(baseStamina * heavyMultiplier);
        }
        else
        {
            currentMaxStamina = baseStamina;
        }
    }

}
