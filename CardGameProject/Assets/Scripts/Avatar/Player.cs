using PixelCrushers.Wrappers;
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
    private TMP_Text _maxStaminaValue;
    private Slider _staminaBar;
    private TMP_Text _staminaValue;
    private TMP_Text _blockValue;
    private Slider _guardBar;
    private TMP_Text _guardValue;

    private GameObject StatusEffectPrefab;
    private GameObject StatusActive;
    private GameObject StatusDeactive;

    private void Awake()
    {
        Animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        OnStatChanged += DisplayStats;
    }

    private void OnDisable()
    {
        OnStatChanged -= DisplayStats;
    }

    public void InitUI(TMP_Text healthValue, TMP_Text maxStaminaValue, TMP_Text staminaValue, TMP_Text blockValue, Slider guardBar, ArmourType armourType, GameObject statusPrefab, GameObject active, GameObject deactive)
    {
        _healthValue = healthValue;
        _staminaValue = staminaValue;
        _maxStaminaValue = maxStaminaValue;
        _guardBar = guardBar;
        _blockValue = blockValue;
        ArmourType = armourType;

        StatusEffectPrefab = statusPrefab;
        StatusActive = active;
        StatusDeactive = deactive;
    }

    public void InitStats(float maxHealth, float maxStamina, int maxGuard, float defence, float defencePercentage, float attack, float blockScale, float recoverStamPercentage)
    {
        MaxHealth = maxHealth;
        MaxGuard = maxGuard;
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

    public override void ResetDeath()
    {
        base.ResetDeath();
        CurrentStamina = _maxStamina;
    }

    private void DisplayStats()
    {
        _currentHealth = Mathf.Clamp(CurrentHealth, 0f, MaxHealth);
        _currentStamina = Mathf.Clamp(CurrentStamina, 0f, _maxStamina);

        _healthValue.text = CurrentHealth.ToString();

        //_staminaBar.value = CurrentStamina / _maxStamina;
        _staminaValue.text = CurrentStamina.ToString();
        _maxStaminaValue.text = _maxStamina.ToString();

        _guardBar.value = (float)CurrentGuard / MaxGuard;
        //_guardValue.text = CurrentGuard.ToString();

        _blockValue.text = CurrentBlock.ToString();

        UpdateStatusEffectsUI();
    }

    public void UpdateStatusEffectsUI()
    {
        for (int i = 0; i < ListOfEffects.Count; i++)
        {
            StatusEffect data = ListOfEffects[i];

            // Check activeParent childs
            GameObject activeObj = GetEffectObject(StatusActive, data.EffectName);
            if (activeObj != null)
            {
                continue;
            }

            // Check deactiveParent childs
            GameObject deactiveObj = GetEffectObject(StatusDeactive, data.EffectName);
            if (deactiveObj != null)
            {
                // Set to active and update effect
                deactiveObj.transform.parent = StatusActive.transform;
                deactiveObj.SetActive(true);
                continue;
            }

            // Instanitate object in active
            GameObject newEffectObj = Instantiate(StatusEffectPrefab, StatusActive.transform);
            newEffectObj.tag = data.EffectName;
            newEffectObj.GetComponent<Image>().sprite = data.Sprite;
        }
    }

    GameObject GetEffectObject(GameObject parentToCheck, string effectName)
    {
        for (int i = 0; i < parentToCheck.transform.childCount; i++)
        {
            if (effectName == parentToCheck.transform.GetChild(i).tag)
            {
                return parentToCheck.transform.GetChild(i).gameObject;
            }
        }

        return null;
    }
}
