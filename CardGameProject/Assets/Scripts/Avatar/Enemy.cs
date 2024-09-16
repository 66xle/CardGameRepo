using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : Avatar
{
    [HideInInspector] public EnemyObj enemyObj;
    private GameObject arrow;

    [Header("References")]
    [HideInInspector] public Slider healthBar;
    [HideInInspector] public TMP_Text healthValue;
    [HideInInspector] public Slider guardBar;
    [HideInInspector] public TMP_Text guardValue;
    [HideInInspector] public TMP_Text blockValue;
    [HideInInspector] public EnemyUI enemyUI;
    private DetailedUI detailedUI;

    [Header("Cards")]
    [SerializeField] float drawAmount;
    public List<Card> deck;
    [HideInInspector] public List<Card> cardsToPlay;

    [HideInInspector] public bool disableSelection;

    private Animator animController;

    // Start is called before the first frame update
    void Start()
    {
        isInCounterState = false;
        disableSelection = false;

        animController = GetComponent<Animator>();
    }

    public void InitStats(GameObject statsUI, DetailedUI detailedUI)
    {
        healthBar = statsUI.GetComponentsInChildren<Slider>()[0];
        guardBar = statsUI.GetComponentsInChildren<Slider>()[1];
        healthValue = statsUI.GetComponentsInChildren<TMP_Text>()[0];
        guardValue = statsUI.GetComponentsInChildren<TMP_Text>()[1];
        blockValue = statsUI.GetComponentsInChildren<TMP_Text>()[2];
        enemyUI = statsUI.GetComponent<EnemyUI>();
        this.detailedUI = detailedUI;


        currentHealth = maxHealth;
        currentGuard = maxGuard;
        DisplayStats();
    }

    public void Init(EnemyObj obj)
    {
        enemyObj = obj;
        deck = enemyObj.cardList;
        maxHealth = enemyObj.health;

        arrow = transform.GetChild(0).gameObject; // Grab arrow object (temp)
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

    public override void RecoverGuardBreak()
    {
        animController.SetBool("isStunned", false);
        base.RecoverGuardBreak();
    }

    public override void ApplyGuardBreak(StatusEffect effectObject)
    {
        animController.SetBool("isStunned", true);
        base.ApplyGuardBreak(effectObject);
    }

    public override void DisplayStats()
    {
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

        healthBar.value = currentHealth / maxHealth;
        healthValue.text = currentHealth.ToString() + " / " + maxHealth.ToString();

        guardBar.value = (float)currentGuard / maxGuard;
        guardValue.text = currentGuard.ToString() + " / " + maxGuard.ToString();

        blockValue.text = currentBlock.ToString();

        detailedUI.DisplayStats();
        detailedUI.UpdateEffectUI();
    }

    public void EnemySelection(bool toggle)
    {
        if (disableSelection)
            return;
        
        arrow.SetActive(toggle);
        enemyUI.SetUIActive(toggle);

        if (toggle)
        {
            detailedUI.ChangeTarget(this);
        }
    }
}
