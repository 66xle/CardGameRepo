using System.Collections;
using System.Collections.Generic;
using TMPro;
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
    public TMP_Text healthValue;
    public Slider staminaBar;
    public TMP_Text staminaValue;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        currentStamina = maxStamina;
        DisplayStats();
    }

    public bool hasEnoughStamina(float cost)
    {
        if (currentStamina >= cost)
        {
            return true;
        }

        return false;
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
        healthValue.text = currentHealth.ToString() + " / " + maxHealth.ToString();

        staminaBar.value = currentStamina / maxStamina;
        staminaValue.text = currentStamina.ToString() + " / " + maxStamina.ToString();
    }
}
