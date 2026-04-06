using MyBox;
using System;
using System.Collections.Generic;
using UnityEngine;

public class BlacksmithUI : MonoBehaviour
{
    [MustBeAssigned][SerializeField] GameObject GearIconPrefab;
    [MustBeAssigned][SerializeField] Transform AllInventoryParent;
    [MustBeAssigned][SerializeField] Transform WeaponInventoryParent;
    [MustBeAssigned][SerializeField] Transform ArmourInventoryParent;
    [MustBeAssigned][SerializeField] Transform AccessoriesInventoryParent;
    [MustBeAssigned][SerializeField] GearSelectionUI GearSelectionUI;

    [MustBeAssigned][SerializeField] Transform SelectedCardParent;

    Action<GearData> _onClickSelectIcon;
    GearData _selectedGear;
    CardContainer _cardContainer;

    [MustBeAssigned][SerializeField] EquipmentManager equipmentManager; // temp for testing
    [MustBeAssigned][SerializeField] CardManager CardManager; // temp for testing

    public void Awake()
    {
        SceneInitialize.Instance.Subscribe(Init);
    }

    void Init()
    {
        _cardContainer = SelectedCardParent.GetComponent<CardContainer>();


        equipmentManager.SaveGear(); // temp for testing

        _onClickSelectIcon += SelectIcon;
        LoadInventory();
    }

    void LoadInventory()
    {
        List<GearData> weapons = new List<GearData>(GameManager.Instance.EquippedWeapons);
        weapons.Add(GameManager.Instance.MainHand);

        List<GearData> armours = new List<GearData>(GameManager.Instance.EquippedArmour);

        // All and weapon tabs
        foreach (GearData data in weapons)
        {
            CreateGearIcon(data, AllInventoryParent);
            CreateGearIcon(data, WeaponInventoryParent);
        }


        // All and Armor tabs
        foreach (GearData data in armours)
        {
            if (data == null) continue;

            CreateGearIcon(data, AllInventoryParent);
            CreateGearIcon(data, ArmourInventoryParent);
        }

        // Missing accessories tab for now

        SelectTab(AllInventoryParent.gameObject);
    }

    private void CreateGearIcon(GearData data, Transform parent)
    {
        GameObject iconObj = Instantiate(GearIconPrefab, parent);
        GearIconUI iconUI = iconObj.GetComponent<GearIconUI>();
        iconUI.SetData(data, _onClickSelectIcon);
    }

    public void SelectTab(GameObject selectedTab)
    {
        AllInventoryParent.gameObject.SetActive(false);
        WeaponInventoryParent.gameObject.SetActive(false);
        ArmourInventoryParent.gameObject.SetActive(false);
        AccessoriesInventoryParent.gameObject.SetActive(false);

        selectedTab.SetActive(true);
    }

    public void SelectIcon(GearData gearData)
    {
        GearSelectionUI.gameObject.SetActive(true);
        GearSelectionUI.SelectGear(gearData);
        _selectedGear = gearData;

        _cardContainer.DestroyAllCards(); // DO OBJECT POOLING

        // spawn in cards
        foreach (CardAnimationData data in gearData.Cards)
        {
            CardData cardData = CardManager.CreateCardData(gearData, data);

            for (int i = 0; i < data.CardAmount; i++)
            {
                CardManager.CreateCard(cardData, SelectedCardParent);
            }
        }
    }
}
