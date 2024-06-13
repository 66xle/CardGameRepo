using System.Collections;
using System.Collections.Generic;
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

    public virtual void TakeDamage(float damage) 
    {
        currentBlock -= damage;

        // Block is negative do damage / Block is positive dont do damage
        damage = currentBlock < 0 ? Mathf.Abs(currentBlock) : 0;

        if (currentBlock < 0) currentBlock = 0;

        currentHealth -= damage;
    }

    public virtual void ReduceGuard()
    {
        currentGuard--;

        // Check guard broken then do effect
    }

    public virtual void AddBlock(float block)
    {
        currentBlock += block;
    }

    public virtual void Heal(float healAmount)
    {
        currentHealth += healAmount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
    }
}
