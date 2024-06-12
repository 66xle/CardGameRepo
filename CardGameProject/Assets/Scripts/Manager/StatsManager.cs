using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatsManager : MonoBehaviour
{
    [SerializeField] float baseHealth;
    [SerializeField] float baseStamina;

    [HideInInspector] public float currentHealth;
    [HideInInspector] public float currentStamina;

    [Header("Armour Stamina")]
    [Range(0, 2)] public float lightMultiplier;
    [Range(0, 2)] public float mediumMultiplier;
    [Range(0, 2)] public float heavyMultiplier;


    void Start() // Initalise variables for now
    {
        currentHealth = baseHealth;
        currentStamina = baseStamina;
    }

}
