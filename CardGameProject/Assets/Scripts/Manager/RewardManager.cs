using MyBox;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEditor.Compilation;
using TMPro;

public class RewardManager : MonoBehaviour
{
    [Foldout("Gear Overlay", true)]
    [MustBeAssigned][SerializeField] GameObject GearOverlay;
    [MustBeAssigned][SerializeField] Transform DisplayRewardsUI;
    [MustBeAssigned][SerializeField] Transform PreviewCards;

    [Foldout("Rewards", true)]
    [ReadOnly][SerializeField] List<WeaponData> PoolOfGear;
    [SerializeField] int LowLevel;
    [SerializeField] int MidLevel;
    [PositiveValueOnly] [SerializeField] Vector3 BattleMultiplier;

    [Foldout("Exp References", true)]
    [MustBeAssigned][SerializeField] Slider ExpSlider;
    [MustBeAssigned][SerializeField] TMP_Text ExpText;
    [MustBeAssigned][SerializeField] TMP_Text LevelText;

    [Foldout("References", true)]
    [MustBeAssigned] [SerializeField] UIManager UIManager;
    [MustBeAssigned] [SerializeField] EquipmentManager EquipmentManager;
    [MustBeAssigned][SerializeField] PlayerStatSettings PSS;
    [MustBeAssigned][SerializeField] LootTable LootTable;
    [MustBeAssigned] [SerializeField] Camera RenderCamera;
    [MustBeAssigned] [SerializeField] GameObject IconPrefab;
    [MustBeAssigned] [SerializeField] GameObject CardPrefab;
    [MustBeAssigned] public GameObject RewardUI;
    [MustBeAssigned] public GameObject GameOverUI;
    

    private List<WeaponData> listOfWeaponReward = new();
    private GameObject currentObjectInOverlay;
    private CardCarousel cardCarousel;

    private void Awake()
    {
        cardCarousel = PreviewCards.GetComponent<CardCarousel>();
    }

    public void ClaimGear()
    {
        EquipmentManager.AddWeapon(listOfWeaponReward[0]);

        EquipmentManager.SaveWeapons();

        UIManager.NextScene();
    }

    public void DisplayReward()
    {
        CalculateExp();

        PoolOfGear.Clear();

        float playerLevel = GameManager.Instance.PlayerLevel;

        if (playerLevel <= LowLevel) 
            PoolOfGear.AddRange(LootTable.CommonGear);
        if (playerLevel > LowLevel && playerLevel <= MidLevel) 
            PoolOfGear.AddRange(LootTable.RareGear);
        if (playerLevel > MidLevel) 
            PoolOfGear.AddRange(LootTable.EpicGear);

        int index = Random.Range(0, PoolOfGear.Count);

        if (PoolOfGear.Count == 0)
        {
            Debug.LogAssertion("No weapon rewards in list");
            return;
        }

        WeaponData weaponData = PoolOfGear[index];
        listOfWeaponReward.Add(weaponData);

        GameObject icon = Instantiate(IconPrefab, DisplayRewardsUI);
        RawImage image = icon.GetComponent<RawImage>();
        image.texture = weaponData.IconTexture;

        Button button = icon.GetComponent<Button>();
        button.onClick.AddListener(() => OpenGearOverlay(weaponData));

        GearItem item = icon.GetComponent<GearItem>();
        item.WeaponData = weaponData;
    }

    public void OpenGearOverlay(WeaponData data)
    {
        RenderCamera.gameObject.SetActive(true);

        Weapon weapon = data.Prefab.GetComponent<Weapon>();

        Vector3 spawnPos = RenderCamera.transform.position + weapon.positionOffset;
        currentObjectInOverlay = Instantiate(data.Prefab, spawnPos, Quaternion.Euler(weapon.rotationOffset));

        foreach (WeaponCardData weaponCardData in data.Cards)
        {
            CardData cardData = new(data, weaponCardData, data.WeaponAttack);

            CardDisplay cardDisplay = Instantiate(CardPrefab, PreviewCards).GetComponent<CardDisplay>();
            cardDisplay.SetCard(cardData.Card);
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

    public void CalculateExp()
    {
        int playerLevel = GameManager.Instance.PlayerLevel;
        float currentExp = GameManager.Instance.CurrentEXP;
        float expNeeded = PSS.CalculateExperience(playerLevel);
        float previousExpNeeded = 0;
        float expDiff = expNeeded;

        if (playerLevel > 1)
        {
            previousExpNeeded = PSS.CalculateExperience(playerLevel - 1);
            expDiff = expNeeded - previousExpNeeded;
        }

        float battleMultiplier = 0;
        if (playerLevel <= LowLevel) battleMultiplier = BattleMultiplier.x;
        if (playerLevel > LowLevel && playerLevel <= MidLevel) battleMultiplier = BattleMultiplier.y;
        if (playerLevel > MidLevel) battleMultiplier = BattleMultiplier.z;

        float expGained = Mathf.Ceil(expDiff * battleMultiplier);

        currentExp += expGained;
        if (currentExp >= expNeeded)
        {
            playerLevel++;
            GameManager.Instance.PlayerLevel = playerLevel;

            expNeeded = PSS.CalculateExperience(playerLevel);
            previousExpNeeded = PSS.CalculateExperience(playerLevel - 1);
            expDiff = expNeeded - previousExpNeeded;
        }

        GameManager.Instance.CurrentEXP = (int)currentExp;

        float current = currentExp - previousExpNeeded;
        float maxExp = expDiff;

        ExpSlider.value = current / maxExp;
        LevelText.text = $"Level {playerLevel}";
        ExpText.text = $"+ {expGained}";
    }
}
