using MyBox;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEditor.Compilation;

public class RewardManager : MonoBehaviour
{
    [Header("References")]
    [MustBeAssigned] [SerializeField] UIManager UIManager;
    [MustBeAssigned] [SerializeField] EquipmentManager EquipmentManager;
    [MustBeAssigned] [SerializeField] Camera RenderCamera;
    [MustBeAssigned] [SerializeField] GameObject IconPrefab;
    [MustBeAssigned] [SerializeField] GameObject CardPrefab;

    [MustBeAssigned] [SerializeField] GameObject GearOverlay;
    [MustBeAssigned] [SerializeField] Transform DisplayRewardsUI;
    [MustBeAssigned] [SerializeField] Transform PreviewCards;

    [Separator]

    [SerializeField] List<WeaponData> PoolOfGear;

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

        foreach (WeaponCardData cardData in data.Cards)
        {
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
}
