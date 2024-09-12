using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public enum ArmourType
{
    None,
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

    // Animation Events
    [HideInInspector] public bool doDamage;
    [HideInInspector] public bool attackFinished;

    [HideInInspector] public bool isInCounterState;

    protected float currentHealth;
    protected float currentBlock = 0f;
    protected int currentGuard;
    [HideInInspector] public List<StatusEffectData> listOfEffects = new List<StatusEffectData>();

    #region Avatar Methods

    public void TakeDamage(float damage) 
    {
        damage = DoExtraDmgCheck(damage);

        currentBlock -= damage;

        // Block is negative do damage / Block is positive dont do damage
        damage = currentBlock < 0 ? Mathf.Abs(currentBlock) : 0;

        if (currentBlock < 0) currentBlock = 0;

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
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

    public void ReduceGuard()
    {
        currentGuard--;
        currentGuard = Mathf.Clamp(currentGuard, 0, maxGuard);
    }

    public bool isGuardBroken()
    {
        return currentGuard == 0 ? true : false;
    }

    public virtual void RecoverGuardBreak()
    {
        currentGuard = maxGuard;
    }

    public virtual void DisplayStats() { }

    #endregion

    #region Apply Status Effects

    public virtual void ApplyGuardBreak(StatusEffect effectObject)
    {
        StatusGuardBroken effect = effectObject as StatusGuardBroken;
        listOfEffects.Add(new StatusEffectData(effect.effect, effect.name, effect.turnsRemaning, effect.uiPrefab, numHitToRecover: effect.numberOfHitsToRecover, extraDmgPer: effect.extraDamagePercentage, nextTurn: true));
    }

    public void ApplyBleed(StatusEffect effectObject)
    {
        StatusBleed effect = effectObject as StatusBleed;
        listOfEffects.Add(new StatusEffectData(effect.effect, effect.name, effect.turnsRemaning, effect.uiPrefab, reduceDmgPer: effect.reduceHealthPercentage));
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

    public void ReduceHealth(float percentage)
    {
        currentHealth -= maxHealth * percentage;
    }

    public float DoExtraDmgCheck(float damage)
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

    public bool IsAvatarDead()
    {
        if (currentHealth <= 0f)
        {
            return true;
        }

        return false;
    }

    #endregion

    public void AnimationEventAttack()
    {
        doDamage = true;
    }

    public void AnimationEventAttackFinish()
    {
        attackFinished = true;
    }
}
