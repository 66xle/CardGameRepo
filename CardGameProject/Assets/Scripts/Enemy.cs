using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : Avatar
{
    [HideInInspector] public EnemyObj enemyObj;

    [Header("References")]
    private Slider healthBar;
    private TMP_Text healthValue;
    private Slider guardBar;
    private TMP_Text guardValue;
    private TMP_Text blockValue;

    [Header("Cards")]
    [SerializeField] float drawAmount;
    public List<Card> deck;
    [HideInInspector] public List<Card> cardsToPlay;
    


    // Start is called before the first frame update
    void Start()
    {
        healthBar = GetComponentsInChildren<Slider>()[0];
        guardBar = GetComponentsInChildren<Slider>()[1];
        blockValue = GetComponentsInChildren<TMP_Text>()[0];
        healthValue = GetComponentsInChildren<TMP_Text>()[1];
        guardValue = GetComponentsInChildren<TMP_Text>()[2];

        currentHealth = maxHealth;
        currentGuard = maxGuard;
        DisplayStats();
    }

    public bool isDead()
    {
        if (currentHealth <= 0f)
            return true;

        return false;
    }

    public List<Card> DrawCards()
    {
        List<Card> cardDrawn = new List<Card>();

        for (int i = 0; i < drawAmount; i++)
        {
            int index = Random.Range(0, deck.Count);
            cardDrawn.Add(deck[index]);
        }

        return cardDrawn;
    }

    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);
        DisplayStats();
    }

    public override void ReduceGuard()
    {
        base.ReduceGuard();
        DisplayStats();
    }
    public override void AddBlock(float block)
    {
        base.AddBlock(block);
        DisplayStats();
    }

    public override void Heal(float healAmount)
    {
        base.Heal(healAmount);
        DisplayStats();
    }

    public void DisplayStats()
    {
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

        healthBar.value = currentHealth / maxHealth;
        healthValue.text = currentHealth.ToString() + " / " + maxHealth.ToString();

        guardBar.value = (float)currentGuard / maxGuard;
        guardValue.text = currentGuard.ToString() + " / " + maxGuard.ToString();

        blockValue.text = currentBlock.ToString();
    }
}
