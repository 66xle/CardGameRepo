using UnityEngine;



public class StatsManager : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] float BaseHealth;
    [SerializeField] float BaseStamina;
    [SerializeField] int BaseGuard;
    public ArmourType ArmourType;

    [Header("Armour Stamina")]
    [Range(0, 2)] public float LightMultiplier;
    [Range(0, 2)] public float MediumMultiplier;
    [Range(0, 2)] public float HeavyMultiplier;

    public float CurrentMaxHealth { get; private set; }
    public float CurrentMaxStamina { get; private set; }
    public int CurrentMaxGuard { get; private set; }


    void Awake() // Initalise variables for now
    {
        CurrentMaxHealth = BaseHealth;
        CurrentMaxGuard = BaseGuard;

        if (ArmourType == ArmourType.Light)
        {
            CurrentMaxStamina = Mathf.RoundToInt(BaseStamina * LightMultiplier);
        }
        else if (ArmourType == ArmourType.Medium)
        {
            CurrentMaxStamina = Mathf.RoundToInt(BaseStamina * MediumMultiplier);
        }
        else if (ArmourType == ArmourType.Heavy)
        {
            CurrentMaxStamina = Mathf.RoundToInt(BaseStamina * HeavyMultiplier);
        }
        else
        {
            CurrentMaxStamina = BaseStamina;
        }
    }

}
