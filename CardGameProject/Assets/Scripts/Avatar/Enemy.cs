using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : Avatar
{
    [HideInInspector] public EnemyObj enemyObj;

    [Header("References")]
    [HideInInspector] public Image healthBar;
    [HideInInspector] public Image guardBar;
    [HideInInspector] public EnemyUI enemyUI;
    [HideInInspector] public DetailedUI detailedUI;

    [Header("Cards")]
    [SerializeField] float drawAmount;
    public List<Card> deck;
    [HideInInspector] public List<Card> cardsToPlay;
    [HideInInspector] public bool disableSelection;

    private GameObject selectionRing;


    private void OnEnable()
    {
        OnStatChanged += DisplayStats;
    }

    private void OnDisable()
    {
        OnStatChanged -= DisplayStats;
    }

    public void InitStats(GameObject statsUI, DetailedUI detailedUI)
    {
        healthBar = statsUI.GetComponentsInChildren<Image>()[1];
        guardBar = statsUI.GetComponentsInChildren<Image>()[2];
        enemyUI = statsUI.GetComponent<EnemyUI>();
        this.detailedUI = detailedUI;

        CurrentHealth = maxHealth;
        CurrentGuard = maxGuard;
    }

    public void Init(EnemyObj obj)
    {
        disableSelection = false;

        enemyObj = obj;
        deck = enemyObj.cardList;
        maxHealth = enemyObj.health;
        maxGuard = enemyObj.guard;

        selectionRing = transform.GetChild(0).gameObject;
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

    private void DisplayStats()
    {
        _currentHealth = Mathf.Clamp(_currentHealth, 0f, maxHealth);

        healthBar.fillAmount = _currentHealth / maxHealth;

        guardBar.fillAmount = (float)_currentGuard / maxGuard;

        detailedUI.DisplayStats();
        detailedUI.UpdateStatusEffectsUI();
    }

    public void EnemySelection(bool toggle)
    {
        if (disableSelection)
            return;
        
        selectionRing.SetActive(toggle);
        enemyUI.SetUIActive(toggle);

        if (toggle)
        {
            detailedUI.ChangeTarget(this);
        }
    }
}
