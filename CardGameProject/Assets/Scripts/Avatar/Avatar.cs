using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public enum ArmourType
{
    Light,
    Medium,
    Heavy
}

public enum DamageType
{
    Slash,
    Pierce,
    Blunt
}

public class Avatar : MonoBehaviour
{
    [Header("Stats")]
    public float maxHealth = 100f;
    public int maxGuard = 10;
    public ArmourType armourType;
    public DamageType damageType;

    protected float currentHealth;
    protected float currentBlock = 0f;
    protected int currentGuard;
    [HideInInspector] public List<StatusEffectData> listOfEffects = new List<StatusEffectData>();

    #region Avatar Methods

    public void TakeDamage(float damage) 
    {
        currentBlock -= damage;

        // Block is negative do damage / Block is positive dont do damage
        damage = currentBlock < 0 ? Mathf.Abs(currentBlock) : 0;

        if (currentBlock < 0) currentBlock = 0;

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
    }

    public void ReduceGuard()
    {
        currentGuard--;
        currentGuard = Mathf.Clamp(currentGuard, 0, maxGuard);
    }

    public bool isGuardBroken()
    {
        return currentGuard == 0 ? true : false;
    }

    public void AddBlock(float block)
    {
        currentBlock += block;
    }

    public void Heal(float healAmount)
    {
        currentHealth += healAmount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
    }

    public virtual void DisplayStats() { }

    #endregion

    #region Apply Status Effects

    public void ApplyGuardBreak(StatusEffect effectObject)
    {
        StatusGuardBroken effect = effectObject as StatusGuardBroken;

        listOfEffects.Add(new StatusEffectData(effect.effect, effect.name, effect.turnsRemaning, effect.numberOfHitsToRecover, extraDmgPer: effect.extraDamagePercentage));
    }

    public void ApplyBleed(StatusEffect effectObject)
    {
        StatusBleed effect = effectObject as StatusBleed;

        listOfEffects.Add(new StatusEffectData(effect.effect, effect.name, effect.turnsRemaning, reduceDmgPer: effect.reduceHealthPercentage));
    }

    #endregion

    public void ReduceHealth(float percentage)
    {
        currentHealth -= maxHealth * percentage;
    }
}