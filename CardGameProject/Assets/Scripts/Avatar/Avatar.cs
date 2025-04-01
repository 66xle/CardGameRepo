using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.Android.Gradle.Manifest;
using UnityEngine;
using System;
using Action = System.Action;
using System.ComponentModel;
using MyBox;



public class Avatar : MonoBehaviour
{
    [Header("Stats")]
    public float maxHealth = 100f;
    public int maxGuard = 10;
    public ArmourType armourType;
    public DamageType damageType;

    // Animation Events
    [HideInInspector] public bool doDamage;
    [HideInInspector] public bool isAttackFinished;

    [HideInInspector] public bool isInCounterState = false;
    [HideInInspector] public bool isInStatusActivation;
    [HideInInspector] public bool doStatusDmg;

    [HideInInspector] public event Action OnStatChanged;

    [MyBox.ReadOnly] public float _currentHealth;
    protected float _currentBlock;
    protected int _currentGuard;

    protected float CurrentHealth { get { return _currentHealth; } set { _currentHealth = value; UpdateStatsUI(); } }
    protected float CurrentBlock { get { return _currentBlock; } set { _currentBlock = value; UpdateStatsUI(); } }
    protected int CurrentGuard { get { return _currentGuard; } set { _currentGuard = value; UpdateStatsUI(); } }

    [HideInInspector] public List<StatusEffectData> listOfEffects = new List<StatusEffectData>();

    #region Avatar Methods

    public void TakeDamage(float damage) 
    {
        damage = ApplyAdditionalDmgCheck(damage);

        _currentBlock -= damage;

        // Block is negative do damage / Block is positive dont do damage
        damage = _currentBlock < 0 ? Mathf.Abs(_currentBlock) : 0;

        if (_currentBlock < 0) _currentBlock = 0;

        _currentHealth -= damage;
        _currentHealth = Mathf.Clamp(_currentHealth, 0, maxHealth);
    }

    public void TakeDamageByStatusEffect(float damage)
    {
        damage = ApplyAdditionalDmgCheck(damage);

        _currentHealth -= damage;
        _currentHealth = Mathf.Clamp(_currentHealth, 0, maxHealth);
    }

    public bool IsAvatarDead()
    {
        if (_currentHealth <= 0f)
        {
            return true;
        }

        return false;
    }

    public void AddBlock(float block)
    {
        _currentBlock += block;
    }

    public void Heal(float healAmount)
    {
        _currentHealth += healAmount;
        _currentHealth = Mathf.Clamp(_currentHealth, 0, maxHealth);
    }

    public void ReduceGuard()
    {
        _currentGuard--;
        _currentGuard = Mathf.Clamp(_currentGuard, 0, maxGuard);
    }

    public bool isGuardBroken()
    {
        return _currentGuard == 0 ? true : false;
    }

    public virtual void RecoverGuardBreak()
    {
        _currentGuard = maxGuard;
    }

    #endregion

    #region Apply Status Effects

    public virtual void ApplyGuardBreak(StatusEffect effectObject)
    {
        StatusGuardBroken effect = effectObject as StatusGuardBroken;
        listOfEffects.Add(new StatusEffectData(effect.effect, effect.effectName, effect.turnsRemaning, effect.uiPrefab, numHitToRecover: effect.numberOfHitsToRecover, extraDmgPer: effect.extraDamagePercentage, nextTurn: effect.removeEffectNextTurn));
        UpdateStatsUI();
    }

    public void ApplyBleed(StatusEffect effectObject)
    {
        // Check if bleed exists
        for (int i = 0; i < listOfEffects.Count; i++)
        {
            StatusEffectData effectData = listOfEffects[i];

            if (effectData.effect == effectObject.effect)
            {
                StatusBleed overiteEffect = effectObject as StatusBleed;

                listOfEffects[i].turnRemaining = overiteEffect.turnsRemaning;
                listOfEffects[i].stacks++;
                UpdateStatsUI();
                return;
            }
        }
            

        StatusBleed effect = effectObject as StatusBleed;
        listOfEffects.Add(new StatusEffectData(effect.effect, effect.effectName, effect.turnsRemaning, effect.uiPrefab, reduceDmgPer: effect.reduceHealthPercentage, stackable: effect.stackable));
        UpdateStatsUI();
    }

    public void ApplyPoison(StatusEffect effectObject)
    {
        for (int i = 0; i < listOfEffects.Count; i++)
        {
            StatusEffectData effectData = listOfEffects[i];

            if (effectData.effect == effectObject.effect)
            {
                StatusPoison overiteEffect = effectObject as StatusPoison;

                listOfEffects[i].turnRemaining = overiteEffect.turnsRemaning;
                UpdateStatsUI();
                return;
            }
        }

        StatusPoison effect = effectObject as StatusPoison;
        listOfEffects.Add(new StatusEffectData(effect.effect, effect.effectName, effect.turnsRemaning, effect.uiPrefab, reduceDmgPer: effect.reduceHealthPercentage));
        UpdateStatsUI();
    }

    #endregion

    #region Status Effects Methods

    public bool hasStatusEffect(Effect effect)
    {
        foreach (StatusEffectData data in listOfEffects)
        {
            if (data.effect == effect)
            {
                return true;
            }
        }

        return false;
    }

    public void ReduceHealthByPercentage(float percentage, int stacks = 1)
    {
        float damage = maxHealth * (percentage * stacks);
        TakeDamageByStatusEffect(damage);
    }

    public float ApplyAdditionalDmgCheck(float damage)
    {
        // Don't need to check
        if (listOfEffects.Count == 0) return damage;

        foreach (StatusEffectData statusEffect in listOfEffects)
        {
            if (statusEffect.effect == Effect.GuardBroken)
            {
                return damage * statusEffect.extraDamagePercentage + damage;
            }
        }

        return damage;
    }

    #endregion


    public void UpdateStatsUI()
    {
        OnStatChanged?.Invoke();
    }

    public void AnimationEventAttack()
    {
        doDamage = true;
    }

    public void AnimationEventAttackFinish()
    {
        Debug.Log("test");
        isAttackFinished = true;
    }
}
