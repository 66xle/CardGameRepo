using System.Collections.Generic;
using MyBox;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : Avatar
{
    [Header("Cards")]
    [SerializeField] int DrawAmount = 1;
    [ReadOnly] public List<Card> Deck;
    public List<Card> CardsToPlay { get; set; }
    public bool DisableSelection { get; set; }

    [Header("References")]
    public EnemyData EnemyData { get; private set; }
    public Image HealthBar { get; private set; }
    public Image GuardBar { get; private set; }
    public EnemyUI EnemyUI { get; private set; }
    public DetailedUI DetailedUI { get; private set; }
    public GameObject SelectionRing { get; private set; }


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
        HealthBar = statsUI.GetComponentsInChildren<Image>()[1];
        GuardBar = statsUI.GetComponentsInChildren<Image>()[2];
        EnemyUI = statsUI.GetComponent<EnemyUI>();
        DetailedUI = detailedUI;

        CurrentHealth = MaxHealth;
        CurrentGuard = MaxGuard;
    }

    public void Init(EnemyData data)
    {
        DisableSelection = false;

        EnemyData = data;
        Deck = EnemyData.CardList;
        MaxHealth = EnemyData.Health;
        MaxGuard = EnemyData.Guard;

        SelectionRing = transform.GetChild(0).gameObject;
    }
  
    public List<Card> DrawCards()
    {
        List<Card> cardDrawn = new List<Card>();

        for (int i = 0; i < DrawAmount; i++)
        {
            int index = Random.Range(0, Deck.Count);
            cardDrawn.Add(Deck[index]);
        }

        return cardDrawn;
    }

    private void DisplayStats()
    {
        _currentHealth = Mathf.Clamp(_currentHealth, 0f, MaxHealth);

        HealthBar.fillAmount = _currentHealth / MaxHealth;

        GuardBar.fillAmount = (float)_currentGuard / MaxGuard;

        DetailedUI.DisplayStats();
        DetailedUI.UpdateStatusEffectsUI();
    }

    public void EnemySelection(bool toggle)
    {
        if (DisableSelection) return;

        SelectionRing.SetActive(toggle);
        EnemyUI.SetUIActive(toggle);

        if (toggle)
        {
            DetailedUI.ChangeTarget(this);
        }
    }
}
