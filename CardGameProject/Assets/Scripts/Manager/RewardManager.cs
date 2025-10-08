using MyBox;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class RewardManager : MonoBehaviour
{
    [Foldout("Gear Overlay", true)]
    [MustBeAssigned][SerializeField] GameObject GearOverlay;
    [MustBeAssigned][SerializeField] Transform DisplayRewardsUI;
    [MustBeAssigned][SerializeField] Transform PreviewCards;

    [Foldout("Gear Select", true)]
    [MustBeAssigned][SerializeField] GameObject GearPrefab;
    [MustBeAssigned][SerializeField] Transform GearParent;

    [Foldout("Rewards", true)]
    [ReadOnly][SerializeField] List<GearData> PoolOfGear;
    [SerializeField] int LowLevel;
    [SerializeField] int MidLevel;
    [PositiveValueOnly] [SerializeField] Vector3 BattleMultiplier;

    [Foldout("Victory UI", true)]
    [MustBeAssigned][SerializeField] GameObject VictoryObject;
    [MustBeAssigned][SerializeField] CanvasGroup CanvasGroup;
    [MustBeAssigned][SerializeField] float AlphaDuration;
    [MustBeAssigned][SerializeField] Image ExpFill;

    [Foldout("Exp References", true)]
    [MustBeAssigned][SerializeField] Slider ExpSlider;
    [MustBeAssigned][SerializeField] TMP_Text ExpText;
    [MustBeAssigned][SerializeField] TMP_Text LevelText;
    [MustBeAssigned][SerializeField] float AnimationTime = 2f;
    [MustBeAssigned][SerializeField] AudioData ExpGainSound;
    [MustBeAssigned][SerializeField] AudioData LeveUpSound;

    [Foldout("References", true)]
    [MustBeAssigned] [SerializeField] CombatStateMachine Ctx;
    [MustBeAssigned] [SerializeField] UIManager UIManager;
    [MustBeAssigned] [SerializeField] EquipmentManager EquipmentManager;
    [MustBeAssigned] [SerializeField] StatsManager StatsManager;
    [MustBeAssigned] [SerializeField] DifficultyManager DifficultyManager;
    [MustBeAssigned] [SerializeField] CutsceneManager CutsceneManager;
    [MustBeAssigned][SerializeField] PlayerStatSettings PSS;
    [MustBeAssigned][SerializeField] LootTable LootTable;
    [MustBeAssigned] [SerializeField] Camera RenderCamera;
    [MustBeAssigned] [SerializeField] GameObject IconPrefab;
    [MustBeAssigned] [SerializeField] GameObject CardPrefab;
    [MustBeAssigned] public GameObject RewardUI;
    [MustBeAssigned] public GameObject GameOverUI;
    [MustBeAssigned] public GameObject ChooseGearUI;
    
    public List<GearData> ListOfRewards = new();
    private GameObject currentObjectInOverlay;
    private CardCarousel cardCarousel;
    private GearSelect selectedGear;

    private void Awake()
    {
        SceneInitialize.Instance.Subscribe(Init);
    }

    private void Init()
    {
        cardCarousel = PreviewCards.GetComponent<CardCarousel>();
    }

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    Time.timeScale = 0f;
        //    DisplayVictoryUI();
        //}
    }

    public void RewardConfirmButton()
    {
        if (ListOfRewards.Count > 0)
        {
            EquipmentManager.AddGear(ListOfRewards[0]);
            EquipmentManager.SaveGear();
        }

        Ctx.EndGameplay();
        ListOfRewards.Clear();

        LevelData levelData = GameManager.Instance.CurrentLevelDataLoaded;

        if (levelData.IsFixed)
        {
            if (!levelData.IsWaveLimitReached(DifficultyManager.WaveCount))
            {
                Time.timeScale = 1;

                DifficultyManager.WaveCount++;
                RewardUI.SetActive(false);

                CutsceneManager.NextCutscene();


                return;
            }
        }

        Time.timeScale = 1;
        RewardUI.SetActive(false);
        CutsceneManager.NextCutscene();
    }

    public void DisplayVictoryUI()
    {
        DetermineDrops();

        if (PoolOfGear.Count == 0)
        {
            Debug.LogAssertion("No weapon rewards in list");
            return;
        }

        // Determine weapon or armor drop

        List<GearData> gears = new();

        for (int i = 0; i < 3; i++)
        {
            int index = Random.Range(0, PoolOfGear.Count); // Not the same weapon. And remove weapon from pool if chosen.
            gears.Add(PoolOfGear[index]);
        }

        //ChooseGearUI.SetActive(true);
        //CreateGearItem(gears);

        // Play animations
        VictoryObject.SetActive(true);
        Tween victoryTween = DOVirtual.Float(0, 1, AlphaDuration, a => CanvasGroup.alpha = a).SetUpdate(true);

        CalculateExp(victoryTween);
    }

    public void DetermineDrops()
    {
        PoolOfGear.Clear();

        float playerLevel = GameManager.Instance.PlayerLevel;

        if (playerLevel <= LowLevel)
            PoolOfGear.AddRange(LootTable.CommonGear);
        if (playerLevel > LowLevel && playerLevel <= MidLevel)
            PoolOfGear.AddRange(LootTable.RareGear);
        if (playerLevel > MidLevel)
            PoolOfGear.AddRange(LootTable.EpicGear);
    }

    public void DisplayRewardUI()
    {
        RewardUI.SetActive(true);
    }


    // Choose Gear UI
    public void CreateGearItem(List<GearData> gears)
    {
        foreach (GearData data in gears)
        {
            GameObject gear = Instantiate(GearPrefab, GearParent);
            GearSelect gearSelect = gear.GetComponent<GearSelect>();

            gearSelect.Init(data);

            Button button = gear.GetComponent<Button>();
            button.onClick.AddListener(() => SelectGear(gearSelect));
        }
    }

    public void SelectGear(GearSelect gear)
    {
        if (selectedGear != null)
            selectedGear.ToggleHighlight(false);

        selectedGear = gear;
        selectedGear.ToggleHighlight(true);
    }

    public void ConfirmGearButton()
    {
        ListOfRewards.Add(selectedGear.GetGearData());

        ChooseGearUI.SetActive(false);

        RewardUI.SetActive(true);
        CreateGearIcon();

        //CalculateExp();
    }

    public void CreateGearIcon()
    {
        foreach (GearData data in ListOfRewards)
        {
            GameObject icon = Instantiate(IconPrefab, DisplayRewardsUI);
            RawImage image = icon.GetComponent<RawImage>();
            image.texture = data.IconTexture;

            Button button = icon.GetComponent<Button>();
            button.onClick.AddListener(() => OpenGearOverlay(data));

            GearItem item = icon.GetComponent<GearItem>();
            item.GearData = data;
        }
    }


    // Gear Overlay UI
    public void OpenGearOverlay(GearData data)
    {
        RenderCamera.gameObject.SetActive(true);

        if (data.Prefab == null)
        {
            Debug.LogError($"[Missing Reference] Prefab not set in gear: {data.GearName}");
            return;
        }

        Weapon weapon = data.Prefab.GetComponent<Weapon>();

        Vector3 spawnPos = RenderCamera.transform.position + weapon.positionOffset;
        currentObjectInOverlay = Instantiate(data.Prefab, spawnPos, Quaternion.Euler(weapon.rotationOffset));

        foreach (CardAnimationData animationData in data.Cards)
        {
            if (animationData.Card == null)
            {
                Debug.LogError($"[Missing Reference] Card not set in gear: {data.GearName}");
                return;
            }

            CardData cardData = new(data, animationData, StatsManager.Attack, StatsManager.Defence, StatsManager.BlockScale, StatsManager.CurrentMaxHealth);

            CardDisplay cardDisplay = Instantiate(CardPrefab, PreviewCards).GetComponent<CardDisplay>();
            cardDisplay.SetCard(cardData, cardData.Card);
        }

        GearOverlay.SetActive(true);

        cardCarousel.InitCards();
    }

    public void CloseGearOverlay()
    {
        RenderCamera.gameObject.SetActive(false);
        Destroy(currentObjectInOverlay);

        for (int i = PreviewCards.childCount - 1; i >= 0; i--)
        {
            Destroy(PreviewCards.GetChild(i).gameObject);
        }

        GearOverlay.SetActive(false);
    }



    public void CalculateExp(Tween victoryTween)
    {
        int playerLevel = GameManager.Instance.PlayerLevel;
        float currentExp = GameManager.Instance.CurrentEXP;
        float expNeeded = PSS.CalculateExperience(playerLevel);
        float previousExpNeeded = 0;
        float expNeededToLvlUp = expNeeded;

        if (playerLevel > 1)
        {
            previousExpNeeded = PSS.CalculateExperience(playerLevel - 1);
            expNeededToLvlUp = expNeeded - previousExpNeeded;
        }

        float battleMultiplier = 0;
        if (playerLevel <= LowLevel) battleMultiplier = BattleMultiplier.x;
        if (playerLevel > LowLevel && playerLevel <= MidLevel) battleMultiplier = BattleMultiplier.y;
        if (playerLevel > MidLevel) battleMultiplier = BattleMultiplier.z;

        float expGained = Mathf.Ceil(expNeededToLvlUp * battleMultiplier);

        GameManager.Instance.CurrentEXP = (int)(currentExp + expGained);
        CheckLevelUp();


        // Display current exp
        float current = currentExp - previousExpNeeded;
        float maxExp = expNeededToLvlUp;
        ExpFill.fillAmount = current / expNeededToLvlUp;

        // Increase current exp
        victoryTween.OnComplete(() =>
        {
            AudioManager.Instance.PlaySound(ExpGainSound);

            // Tween animations
            DOVirtual.Float(currentExp, currentExp + expGained, AnimationTime, v =>
            {
                if (v >= expNeeded)
                {
                    playerLevel++;
                    LevelText.text = $"Level {playerLevel}";
                    AudioManager.Instance.PlaySound(LeveUpSound);

                    // Get max exp
                    expNeeded = PSS.CalculateExperience(playerLevel);
                    previousExpNeeded = PSS.CalculateExperience(playerLevel - 1);
                    expNeededToLvlUp = expNeeded - previousExpNeeded;
                }

                float current = v - previousExpNeeded;
                float maxExp = expNeededToLvlUp;
                ExpFill.fillAmount = current / maxExp;
                //ExpSlider.value = current / maxExp;

            }).SetUpdate(true);

            DOVirtual.Int(0, (int)expGained, AnimationTime, v => ExpText.text = $"+ {v.ToString()}").SetUpdate(true).OnComplete(() =>
            {
                VictoryObject.SetActive(false);
                DisplayRewardUI();
            });
        });
    }

    public void CheckLevelUp()
    {
        int playerLevel = GameManager.Instance.PlayerLevel;
        float currentExp = GameManager.Instance.CurrentEXP;
        float expNeeded = PSS.CalculateExperience(playerLevel);

        if (currentExp >= expNeeded)
        {
            GameManager.Instance.PlayerLevel = playerLevel + 1;

            CheckLevelUp();
        }
    }
}
