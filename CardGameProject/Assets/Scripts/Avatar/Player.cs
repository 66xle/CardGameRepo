using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Player : Avatar
{
    private float maxStamina = 5f;
    [SerializeField] float recoverStaminaAmount = 2f;

    private float _currentStamina;
    [HideInInspector] public float CurrentStamina { get { return _currentStamina; } set { _currentStamina = value; UpdateStatsUI(); } }

    [Header("References")]
    private Slider healthBar;
    private TMP_Text healthValue;
    private Slider staminaBar;
    private TMP_Text staminaValue;
    private TMP_Text blockValue;
    private Slider guardBar;
    private TMP_Text guardValue;
    private Animator animController;

    private void OnEnable()
    {
        OnStatChanged += DisplayStats;
    }

    private void OnDisable()
    {
        OnStatChanged -= DisplayStats;
    }

    public void Init(Slider healthBar, TMP_Text healthValue, Slider staminaBar, TMP_Text staminaValue, TMP_Text blockValue, 
                     Slider guardBar, TMP_Text guardValue,
                     ArmourType armourType)
    {
        this.healthBar = healthBar;
        this.healthValue = healthValue;
        this.staminaBar = staminaBar;
        this.staminaValue = staminaValue;
        this.guardBar = guardBar;
        this.guardValue = guardValue;
        this.blockValue = blockValue;
        this.armourType = armourType;

        animController = GetComponent<Animator>();
    }

    public void InitStats(float maxHealth, float maxStamina, int maxGuard)
    {
        base.maxHealth = maxHealth;
        base.maxGuard = maxGuard;

        CurrentHealth = maxHealth;
        CurrentGuard = maxGuard;
        CurrentStamina = this.maxStamina;
    }

    public bool hasEnoughStamina(float cost)
    {
        if (CurrentStamina >= cost)
        {
            return true;
        }

        return false;
    }

    public override void RecoverGuardBreak()
    {
        animController.SetBool("isStunned", false);
        base.RecoverGuardBreak();
    }

    public override void ApplyGuardBreak(StatusEffect effectObject)
    {
        animController.SetBool("isStunned", true);
        base.ApplyGuardBreak(effectObject);
    }


    #region Play Cards

    public void RecycleCardToStamina(float cost)
    {
        CurrentStamina += cost;
        CurrentStamina = Mathf.Clamp(CurrentStamina, 0f, maxStamina);
    }

    public void RecoverStamina()
    {
        CurrentStamina += recoverStaminaAmount;
        CurrentStamina = Mathf.Clamp(CurrentStamina, 0f, maxStamina);
    }

    public void ConsumeStamina(float stamAmount)
    {
        CurrentStamina -= stamAmount;
        CurrentStamina = Mathf.Clamp(CurrentStamina, 0f, maxStamina);
    }

    #endregion

    private void DisplayStats()
    {
        _currentStamina = Mathf.Clamp(_currentHealth, 0f, maxHealth);
        _currentStamina = Mathf.Clamp(_currentStamina, 0f, maxStamina);

        healthBar.value = _currentHealth / maxHealth;
        healthValue.text = _currentHealth.ToString();

        staminaBar.value = _currentStamina / maxStamina;
        staminaValue.text = _currentStamina.ToString();

        guardBar.value = (float)_currentGuard / maxGuard;
        guardValue.text = _currentGuard.ToString();

        blockValue.text = _currentBlock.ToString();
    }

}
