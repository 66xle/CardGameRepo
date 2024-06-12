using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ArmourType
{
    Light,
    Medium,
    Heavy
}

public class Avatar : MonoBehaviour
{
    [Header("Stats")]
    public float maxHealth = 100f;
    public ArmourType armourType = ArmourType.Light;

    protected float currentHealth;
    protected float currentBlock = 0f;



    public virtual void TakeDamage(float damage) 
    {
        currentBlock -= damage;

        // Block is negative do damage / Block is positive dont do damage
        damage = currentBlock < 0 ? Mathf.Abs(currentBlock) : 0;

        if (currentBlock < 0) currentBlock = 0;

        currentHealth -= damage;
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
