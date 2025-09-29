using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Player : Avatar
{
    [SerializeField] float RecoverStaminaPercentage = 2f;
    private float _maxStamina = 5f;
    private float _currentStamina;

    public float CurrentStamina { get { return _currentStamina; } set { _currentStamina = value; UpdateStatsUI(); } }

    [Header("References")]
    private Slider _healthBar;
    private TMP_Text _healthValue;
    private Slider _staminaBar;
    private TMP_Text _staminaValue;
    private TMP_Text _blockValue;
    private Slider _guardBar;
    private TMP_Text _guardValue;

    private void OnEnable()
    {
        OnStatChanged += DisplayStats;
    }

    private void OnDisable()
    {
        OnStatChanged -= DisplayStats;
    }

    //public void InitUI(Slider healthBar, TMP_Text healthValue, Slider staminaBar, TMP_Text staminaValue, TMP_Text blockValue, 
    //                 Slider guardBar, TMP_Text guardValue,
    //                 ArmourType armourType)
    //{
    //    _healthBar = healthBar;
    //    _healthValue = healthValue;
    //    _staminaBar = staminaBar;
    //    _staminaValue = staminaValue;
    //    _guardBar = guardBar;
    //    _guardValue = guardValue;
    //    _blockValue = blockValue;
    //    ArmourType = armourType;
    //}

    public void InitUI(TMP_Text healthValue, TMP_Text staminaValue, TMP_Text blockValue, Slider guardBar, ArmourType armourType)
    {
        _healthValue = healthValue;
        _staminaValue = staminaValue;
        _guardBar = guardBar;
        _blockValue = blockValue;
        ArmourType = armourType;
    }

    public void InitStats(float maxHealth, float maxStamina, int maxGuard, float defence, float defencePercentage, float attack, float blockScale, float recoverStamPercentage)
    {
        base.MaxHealth = maxHealth;
        base.MaxGuard = maxGuard;
        _maxStamina = maxStamina;

        CurrentHealth = maxHealth;
        CurrentGuard = maxGuard;
        CurrentStamina = _maxStamina;

        Defence = defence;
        DefencePercentage = defencePercentage;
        Attack = attack;
        BlockScale = blockScale;
        RecoverStaminaPercentage = recoverStamPercentage; 
    }

    public bool hasEnoughStamina(float cost)
    {
        if (CurrentStamina >= cost)
        {
            return true;
        }

        return false;
    }


    #region Play Cards

    public void RecycleCardToStamina(float cost)
    {
        CurrentStamina = CurrentStamina + cost;
        CurrentStamina = Mathf.Clamp(CurrentStamina, 0f, _maxStamina);
    }

    public void RecoverStamina()
    {
        CurrentStamina = CurrentStamina + Mathf.Floor(RecoverStaminaPercentage * _maxStamina);
        CurrentStamina = Mathf.Clamp(CurrentStamina, 0f, _maxStamina);
    }

    public void ConsumeStamina(float stamAmount)
    {
        CurrentStamina = CurrentStamina - stamAmount;
        CurrentStamina = Mathf.Clamp(CurrentStamina, 0f, _maxStamina);
    }

    #endregion

    private void DisplayStats()
    {
        _currentHealth = Mathf.Clamp(CurrentHealth, 0f, MaxHealth);
        _currentStamina = Mathf.Clamp(CurrentStamina, 0f, _maxStamina);

        //_healthBar.value = CurrentHealth / MaxHealth;
        _healthValue.text = CurrentHealth.ToString();

        //_staminaBar.value = CurrentStamina / _maxStamina;
        _staminaValue.text = CurrentStamina.ToString();

        _guardBar.value = (float)CurrentGuard / MaxGuard;
        //_guardValue.text = CurrentGuard.ToString();

        _blockValue.text = CurrentBlock.ToString();
    }

}
