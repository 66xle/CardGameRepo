using UnityEngine;



public class StatsManager : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] float BaseHealth;
    [SerializeField] float BaseStamina;
    [SerializeField] int BaseGuard;
    public ArmourType armourType;

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

        if (armourType == ArmourType.Light)
        {
            CurrentMaxStamina = Mathf.RoundToInt(BaseStamina * LightMultiplier);
        }
        else if (armourType == ArmourType.Medium)
        {
            CurrentMaxStamina = Mathf.RoundToInt(BaseStamina * MediumMultiplier);
        }
        else if (armourType == ArmourType.Heavy)
        {
            CurrentMaxStamina = Mathf.RoundToInt(BaseStamina * HeavyMultiplier);
        }
        else
        {
            CurrentMaxStamina = BaseStamina;
        }
    }

}
