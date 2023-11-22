using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] float maxHealth = 100f;
    private float currentHealth;

    private Slider healthBar;
    private TMP_Text healthValue;

    [Header("Cards")]
    [SerializeField] List<Card> deck;
    [SerializeField] float drawAmount;
    [HideInInspector] public List<Card> cardsToPlay;


    // Start is called before the first frame update
    void Start()
    {
        healthBar = GetComponentInChildren<Slider>();
        healthValue = GetComponentInChildren<TMP_Text>();

        currentHealth = maxHealth;
        DisplayHealth();
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

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        DisplayHealth();
    }
    

    public void DisplayHealth()
    {
        healthBar.value = currentHealth / maxHealth;
        healthValue.text = currentHealth.ToString() + " / " + maxHealth.ToString();
    }
}
