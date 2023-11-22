using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] float maxHealth = 100f;
    private float currentHealth;

    [SerializeField] float maxStamina = 5f;
    [HideInInspector] public float currentStamina;

    [Header("References")]
    public Slider healthBar;
    public Slider staminaBar;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        currentStamina = maxStamina;
        DisplayStats();
    }
    public void ConsumeStamina(float stamAmount)
    {
        currentStamina -= stamAmount;
        DisplayStats();
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        DisplayStats();
    }

    public void DisplayStats()
    {
        healthBar.value = currentHealth / maxHealth;
        staminaBar.value = currentStamina / maxStamina;
    }
}
