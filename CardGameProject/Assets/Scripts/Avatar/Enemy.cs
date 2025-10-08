using System.Collections.Generic;
using System.Linq;
using MyBox;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : Avatar
{
    [Header("Cards")]
    [SerializeField] int DrawAmount = 1;
    [ReadOnly] public List<CardData> Deck;

    public List<Card> CardsToPlay { get; set; }
    public bool DisableSelection { get; set; }

    [Header("References")]
    public EnemyData EnemyData { get; private set; }
    public TMP_Text HealthText { get; private set; }
    public TMP_Text BlockText { get; private set; }
    public TMP_Text NameText { get; private set; }
    public Slider GuardBar { get; private set; }
    public EnemyUI EnemyUI { get; private set; }
    public DetailedUI DetailedUI { get; private set; }
    public GameObject SelectionRing { get; private set; }
    public bool HasDialogue { get; private set; }

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
        EnemyUI = statsUI.GetComponent<EnemyUI>();
        HealthText = EnemyUI.HealthText;
        GuardBar = EnemyUI.GuardBar;
        BlockText = EnemyUI.BlockText;

        if (EnemyUI.Name != null)
            EnemyUI.Name.text = EnemyData.Name;


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

        HasDialogue = false;

        if (data.EnemyType == EnemyType.Elite && data.HasDialogue)
            HasDialogue = true;


        Deck = new();
        Deck.AddRange(data.Cards.Select(card => new CardData(weapon, card, Attack, Defence, BlockScale, MaxHealth)));

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

        HealthText.text = CurrentHealth.ToString();
        GuardBar.value = (float)CurrentGuard / MaxGuard;
        BlockText.text = CurrentBlock.ToString();

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

    public void EnableSelectionRing(bool toggle)
    {
        SelectionRing.SetActive(toggle);
    }

    public override void PlayHurtSound()
    {
        AudioManager.Instance.PlaySound(EnemyData.HurtSounds);
    }

    public override void PlayDeathSound()
    {
        AudioManager.Instance.PlaySound(EnemyData.DeathSounds);
    }
}
