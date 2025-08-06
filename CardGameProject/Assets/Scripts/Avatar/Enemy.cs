using System.Collections.Generic;
using System.Linq;
using MyBox;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.GPUSort;

public class Enemy : Avatar
{
    [Header("Cards")]
    [SerializeField] int DrawAmount = 1;
    [ReadOnly] public List<CardData> Deck;
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

    public void InitUI(GameObject statsUI, DetailedUI detailedUI)
    {
        HealthBar = statsUI.GetComponentsInChildren<Image>()[1];
        GuardBar = statsUI.GetComponentsInChildren<Image>()[2];
        EnemyUI = statsUI.GetComponent<EnemyUI>();
        DetailedUI = detailedUI;

        CurrentHealth = MaxHealth;
        CurrentGuard = MaxGuard;
    }

    public void InitStats(EnemyData data, EnemyStatSettings ess)
    {
        DisableSelection = false;

        WeaponData weapon = new();
        weapon.DamageType = DamageType;

        EnemyData = data;
        MaxGuard = data.Guard;
        MaxHealth = ess.CalculateHealth(data.Level, data.EnemyType);
        Attack = ess.CalculateAttack(data.Level, data.EnemyType);
        Defence = ess.CalculateDefence(data.Level, data.EnemyType);
        DefencePercentage = ess.GetDefencePercentage();
        BlockScale = ess.GetBlockScale();

        Deck = new();
        Deck.AddRange(data.Cards.Select(card => new CardData(weapon, card, Attack, Defence, BlockScale)));

        SelectionRing = transform.GetChild(0).gameObject;
    }
  
    public List<CardData> DrawCards()
    {
        List<CardData> cardDrawn = new();

        for (int i = 0; i < DrawAmount; i++)
        {
            int index = Random.Range(0, Deck.Count);
            cardDrawn.Add(Deck[index]);
        }

        return cardDrawn;
    }
    
    private void DisplayStats()
    {
        _currentHealth = Mathf.Clamp(CurrentHealth, 0f, MaxHealth);

        HealthBar.fillAmount = CurrentHealth / MaxHealth;

        GuardBar.fillAmount = (float)CurrentGuard / MaxGuard;

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
