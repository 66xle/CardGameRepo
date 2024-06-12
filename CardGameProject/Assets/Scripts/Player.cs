using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Player : Avatar
{
    private float maxStamina = 5f;
    [SerializeField] float recoverStaminaAmount = 2f;
    [HideInInspector] public float currentStamina;

    [Header("References")]
    private Slider healthBar;
    private TMP_Text healthValue;
    private Slider staminaBar;
    private TMP_Text staminaValue;
    private TMP_Text blockValue;

    public void Init(Slider healthBar, TMP_Text healthValue, Slider staminaBar, TMP_Text staminaValue, TMP_Text blockValue, float currentHealth, float currentStamina)
    {
        this.healthBar = healthBar;
        this.healthValue = healthValue;
        this.staminaBar = staminaBar;
        this.staminaValue = staminaValue;
        this.blockValue = blockValue;

        maxHealth = currentHealth;
        maxStamina = currentStamina;

        this.currentHealth = maxHealth;
        this.currentStamina = maxStamina;

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

    #region Play Cards

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

    #endregion

    #region Card Effects

    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);
        DisplayStats();
    }

    public override void AddBlock(float block)
    {
        base.AddBlock(block);
        DisplayStats();
    }

    public override void Heal(float healAmount)
    {
        base.Heal(healAmount);
        DisplayStats();
    }

    #endregion


    public void DisplayStats()
    {
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
        currentStamina = Mathf.Clamp(currentStamina, 0f, maxStamina);

        healthBar.value = currentHealth / maxHealth;
        healthValue.text = currentHealth.ToString() + " / " + maxHealth.ToString();

        staminaBar.value = currentStamina / maxStamina;
        staminaValue.text = currentStamina.ToString() + " / " + maxStamina.ToString();

        blockValue.text = currentBlock.ToString();
    }
}
