using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : Avatar
{
    [HideInInspector] public EnemyObj enemyObj;

    private Slider healthBar;
    private TMP_Text healthValue;
    private TMP_Text blockValue;

    [Header("Cards")]
    [SerializeField] float drawAmount;
    public List<Card> deck;
    [HideInInspector] public List<Card> cardsToPlay;
    


    // Start is called before the first frame update
    void Start()
    {
        healthBar = GetComponentInChildren<Slider>();
        blockValue = GetComponentsInChildren<TMP_Text>()[0];
        healthValue = GetComponentsInChildren<TMP_Text>()[1];

        currentHealth = maxHealth;
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

    public override void AddBlock(float block)
    {
        base.AddBlock(block);
        DisplayStats();
    }
    public void DisplayStats()
    {
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

        healthBar.value = currentHealth / maxHealth;
        healthValue.text = currentHealth.ToString() + " / " + maxHealth.ToString();

        blockValue.text = currentBlock.ToString();
    }
}
