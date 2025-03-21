using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;



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

    [HideInInspector] public bool isInCounterState;
    [HideInInspector] public bool isInStatusActivation;
    [HideInInspector] public bool doStatusDmg;

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
        listOfEffects.Add(new StatusEffectData(effect.effect, effect.effectName, effect.turnsRemaning, effect.uiPrefab, numHitToRecover: effect.numberOfHitsToRecover, extraDmgPer: effect.extraDamagePercentage, nextTurn: effect.removeEffectNextTurn));
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
                return;
            }
        }
            

        StatusBleed effect = effectObject as StatusBleed;
        listOfEffects.Add(new StatusEffectData(effect.effect, effect.effectName, effect.turnsRemaning, effect.uiPrefab, reduceDmgPer: effect.reduceHealthPercentage, stackable: effect.stackable));
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
                return;
            }
        }

        StatusPoison effect = effectObject as StatusPoison;
        listOfEffects.Add(new StatusEffectData(effect.effect, effect.effectName, effect.turnsRemaning, effect.uiPrefab, reduceDmgPer: effect.reduceHealthPercentage));
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

    public void ReduceHealth(float percentage, int stacks = 1)
    {
        currentHealth -= maxHealth * (percentage * stacks);
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
        isAttackFinished = true;
    }
}
