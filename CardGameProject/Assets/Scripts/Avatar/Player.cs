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
    private Slider guardBar;
    private TMP_Text guardValue;

    public void Init(Slider healthBar, TMP_Text healthValue, Slider staminaBar, TMP_Text staminaValue, TMP_Text blockValue, 
                     Slider guardBar, TMP_Text guardValue,
                     float maxHealth, float maxStamina, int maxGuard, 
                     ArmourType armourType, DamageType damageType)
    {
        this.healthBar = healthBar;
        this.healthValue = healthValue;
        this.staminaBar = staminaBar;
        this.staminaValue = staminaValue;
        this.guardBar = guardBar;
        this.guardValue = guardValue;
        this.blockValue = blockValue;
        this.armourType = armourType;
        this.damageType = damageType;

        base.maxHealth = maxHealth;
        this.maxStamina = maxStamina;
        base.maxGuard = maxGuard;

       currentHealth = base.maxHealth;
       currentStamina = this.maxStamina;
       currentGuard = base.maxGuard;

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

    public override void DisplayStats()
    {
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
        currentStamina = Mathf.Clamp(currentStamina, 0f, maxStamina);

        healthBar.value = currentHealth / maxHealth;
        healthValue.text = currentHealth.ToString() + " / " + maxHealth.ToString();

        staminaBar.value = currentStamina / maxStamina;
        staminaValue.text = currentStamina.ToString() + " / " + maxStamina.ToString();

        guardBar.value = (float)currentGuard / maxGuard;
        guardValue.text = currentGuard.ToString() + " / " + maxGuard.ToString();

        blockValue.text = currentBlock.ToString();
    }

    public override void AnimationEventAttack()
    {
        base.AnimationEventAttack();
    }

    public override void AnimationEventAttackFinish()
    {
        base.AnimationEventAttackFinish();
    }
}
