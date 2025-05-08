using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Player : Avatar
{
    [SerializeField] float RecoverStaminaAmount = 2f;
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

    public void Init(Slider healthBar, TMP_Text healthValue, Slider staminaBar, TMP_Text staminaValue, TMP_Text blockValue, 
                     Slider guardBar, TMP_Text guardValue,
                     ArmourType armourType)
    {
        this._healthBar = healthBar;
        this._healthValue = healthValue;
        this._staminaBar = staminaBar;
        this._staminaValue = staminaValue;
        this._guardBar = guardBar;
        this._guardValue = guardValue;
        this._blockValue = blockValue;
        this.ArmourType = armourType;
    }

    public void InitStats(float maxHealth, float maxStamina, int maxGuard)
    {
        base.MaxHealth = maxHealth;
        base.MaxGuard = maxGuard;

        CurrentHealth = maxHealth;
        CurrentGuard = maxGuard;
        CurrentStamina = this._maxStamina;
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
        CurrentStamina += cost;
        CurrentStamina = Mathf.Clamp(CurrentStamina, 0f, _maxStamina);
    }

    public void RecoverStamina()
    {
        CurrentStamina += RecoverStaminaAmount;
        CurrentStamina = Mathf.Clamp(CurrentStamina, 0f, _maxStamina);
    }

    public void ConsumeStamina(float stamAmount)
    {
        CurrentStamina -= stamAmount;
        CurrentStamina = Mathf.Clamp(CurrentStamina, 0f, _maxStamina);
    }

    #endregion

    private void DisplayStats()
    {
        _currentStamina = Mathf.Clamp(_currentHealth, 0f, MaxHealth);
        _currentStamina = Mathf.Clamp(_currentStamina, 0f, _maxStamina);

        _healthBar.value = _currentHealth / MaxHealth;
        _healthValue.text = _currentHealth.ToString();

        _staminaBar.value = _currentStamina / _maxStamina;
        _staminaValue.text = _currentStamina.ToString();

        _guardBar.value = (float)_currentGuard / MaxGuard;
        _guardValue.text = _currentGuard.ToString();

        _blockValue.text = _currentBlock.ToString();
    }

}
