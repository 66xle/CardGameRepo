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
    [SerializeField] float recoverStaminaAmount = 2f;
    [HideInInspector] public float currentStamina;

    [Header("References")]
    public Slider healthBar;
    public TMP_Text healthValue;
    public Slider staminaBar;
    public TMP_Text staminaValue;

    public void Init()
    {
        currentHealth = maxHealth;
        currentStamina = maxStamina;

        DisplayStats();
    }

    public bool isDead()
    {
        if (currentHealth >= maxHealth)
            return true;

        return false;
    }

    public bool hasEnoughStamina(float cost)
    {
        if (currentStamina >= cost)
        {
            return true;
        }

        return false;
    }

    public void RecycleCardToStamina(float cost)
    {
        currentStamina += cost;
        DisplayStats();
    }

    public void RecoverStamina()
    {
        currentStamina += recoverStaminaAmount;
        DisplayStats();
    }

    public void ConsumeStamina(float stamAmount)
    {
        currentStamina -= stamAmount;
        Mathf.Clamp(currentStamina, 0f, maxStamina);

        DisplayStats();
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        DisplayStats();
    }

    public void DisplayStats()
    {
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
        currentStamina = Mathf.Clamp(currentStamina, 0f, maxStamina);

        healthBar.value = currentHealth / maxHealth;
        healthValue.text = currentHealth.ToString() + " / " + maxHealth.ToString();

        staminaBar.value = currentStamina / maxStamina;
        staminaValue.text = currentStamina.ToString() + " / " + maxStamina.ToString();
    }
}
