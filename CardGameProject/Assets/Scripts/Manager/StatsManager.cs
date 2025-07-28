using MyBox;
using UnityEngine;



public class StatsManager : MonoBehaviour
{
    [Header("Stats")]
    [MustBeAssigned] [SerializeField] PlayerStatSettings PSS;

    [SerializeField] int Level = 1;
    public int BaseGuard;
    public ArmourType ArmourType;

    [Header("Armour Stamina")]
    [Range(0, 2)] public float LightMultiplier;
    [Range(0, 2)] public float MediumMultiplier;
    [Range(0, 2)] public float HeavyMultiplier;

    [ReadOnly] public float Defence;
    [ReadOnly] public float Attack;

    public float CurrentMaxHealth { get; private set; }
    public float CurrentMaxStamina { get; private set; }
    public int CurrentMaxGuard { get; private set; }

    void Awake() // Initalise variables for now
    {
        if (GameManager.Instance.PlayerLevel == 0)
        {
            GameManager.Instance.PlayerLevel = Level;
        }

        CurrentMaxHealth = PSS.CalculateHealth(Level);
        Defence = PSS.CalculateDefence(Level);
        Attack = PSS.CalculateAttack(Level);
        CurrentMaxGuard = BaseGuard;

        if (ArmourType == ArmourType.Light)
        {
            CurrentMaxStamina = Mathf.RoundToInt(PSS.CalculateStamina(Level) * LightMultiplier);
        }
        else if (ArmourType == ArmourType.Medium)
        {
            CurrentMaxStamina = Mathf.RoundToInt(PSS.CalculateStamina(Level) * MediumMultiplier);
        }
        else if (ArmourType == ArmourType.Heavy)
        {
            CurrentMaxStamina = Mathf.RoundToInt(PSS.CalculateStamina(Level) * HeavyMultiplier);
        }
        else
        {
            CurrentMaxStamina = PSS.CalculateStamina(Level);
        }
    }

}
