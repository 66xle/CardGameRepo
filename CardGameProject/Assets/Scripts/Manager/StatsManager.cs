using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class StatsManager : MonoBehaviour
{
    [SerializeField] float baseHealth;
    [SerializeField] float baseStamina;
    public ArmourType armourType;

    [HideInInspector] public float currentHealth;
    [HideInInspector] public float currentStamina;

    [Header("Armour Stamina")]
    [Range(0, 2)] public float lightMultiplier;
    [Range(0, 2)] public float mediumMultiplier;
    [Range(0, 2)] public float heavyMultiplier;


    void Start() // Initalise variables for now
    {
        currentHealth = baseHealth;

        if (armourType == ArmourType.Light)
        {
            currentStamina = Mathf.RoundToInt(baseStamina * lightMultiplier);
        }
        else if (armourType == ArmourType.Medium)
        {
            currentStamina = Mathf.RoundToInt(baseStamina * mediumMultiplier);
        }
        else if (armourType == ArmourType.Heavy)
        {
            currentStamina = Mathf.RoundToInt(baseStamina * heavyMultiplier);
        }
    }

}
