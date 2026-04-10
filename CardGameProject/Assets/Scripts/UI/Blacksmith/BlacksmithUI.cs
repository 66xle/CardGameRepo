using MyBox;
using System;
using System.Collections.Generic;
using UnityEngine;

public class BlacksmithUI : MonoBehaviour
{
    [Foldout("Inventory", true)]
    [MustBeAssigned][SerializeField] public GameObject GearIconPrefab;
    [MustBeAssigned][SerializeField] public Transform AllInventoryParent;
    [MustBeAssigned][SerializeField] public Transform WeaponInventoryParent;
    [MustBeAssigned][SerializeField] public Transform ArmourInventoryParent;
    [MustBeAssigned][SerializeField] public Transform AccessoriesInventoryParent;

    [Foldout("Selection", true)]
    private GearSelectionUI _gearSelectionUI;
    [MustBeAssigned][SerializeField] public Transform SelectedCardParent;


    [Foldout("Objects", true)]
    [MustBeAssigned][SerializeField] public GameObject SelectGearUI;
    [MustBeAssigned][SerializeField] public GameObject UpgradeUI;
    [MustBeAssigned][SerializeField] public GameObject SelectionUI;

    [Foldout("References", true)]
    [MustBeAssigned][SerializeField] public EquipmentManager EquipmentManager; // temp for testing
    [MustBeAssigned][SerializeField] public CardManager CardManager;

    Action<GearData> _onClickSelectIcon;
    GearData _selectedGear;
    CardContainer _cardContainer;

    public void Awake()
    {
        SceneInitialize.Instance.Subscribe(Init);
    }

    void Init()
    {
        _cardContainer = SelectedCardParent.GetComponent<CardContainer>();
        _gearSelectionUI = SelectionUI.GetComponent<GearSelectionUI>();


        EquipmentManager.SaveGear(); // temp for testing

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
        if (!SelectionUI.activeSelf)
            SelectionUI.SetActive(true);

        _gearSelectionUI.SelectGear(gearData);
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

    public void SwitchToUpgradeUI()
    {
        SelectGearUI.SetActive(false);
        UpgradeUI.SetActive(true);
    }

    public void SwitchToSelectionUI()
    {
        UpgradeUI.SetActive(false);
        SelectGearUI.SetActive(true);
    }
}
