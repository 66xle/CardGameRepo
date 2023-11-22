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


    // Start is called before the first frame update
    void Start()
    {
        healthBar = GetComponentInChildren<Slider>();
        healthValue = GetComponentInChildren<TMP_Text>();

        currentHealth = maxHealth;
        DisplayHealth();
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
