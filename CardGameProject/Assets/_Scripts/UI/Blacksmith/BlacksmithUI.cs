using System;
using System.Collections.Generic;
using MyBox;
using PixelCrushers;
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
    [MustBeAssigned][SerializeField] public Transform DisabledCardsParent;


    [Foldout("Objects", true)]
    [MustBeAssigned][SerializeField] public GameObject SelectGearUI;
    [MustBeAssigned][SerializeField] public GameObject UpgradeUI;
    [MustBeAssigned][SerializeField] public GameObject SelectionUI;

    [Foldout("References", true)]
    [MustBeAssigned][SerializeField] public EquipmentManager EquipmentManager; // temp for testing
    [MustBeAssigned][SerializeField] public CardManager CardManager;

    Action<GearRuntime> _onClickSelectIcon;
    GearRuntime _selectedGear;
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
        List<GearRuntime> weapons = new List<GearRuntime>(GameManager.Instance.EquippedWeapons);
        weapons.Add(GameManager.Instance.MainHand);

        List<GearRuntime> armours = new List<GearRuntime>(GameManager.Instance.EquippedArmour);

        // All and weapon tabs
        foreach (GearRuntime gearRuntime in weapons)
        {
            CreateGearIcon(gearRuntime, AllInventoryParent);
            CreateGearIcon(gearRuntime, WeaponInventoryParent);
        }


        // All and Armor tabs
        foreach (GearRuntime gearRuntime in armours)
        {
            if (gearRuntime.GearData == null) continue;

            CreateGearIcon(gearRuntime, AllInventoryParent);
            CreateGearIcon(gearRuntime, ArmourInventoryParent);
        }

        // Missing accessories tab for now

        SelectTab(AllInventoryParent.gameObject);
    }

    private void CreateGearIcon(GearRuntime gearRuntime, Transform parent)
    {
        GameObject iconObj = Instantiate(GearIconPrefab, parent);
        GearIconUI iconUI = iconObj.GetComponent<GearIconUI>();
        iconUI.SetData(gearRuntime, _onClickSelectIcon);
    }

    public void SelectTab(GameObject selectedTab)
    {
        AllInventoryParent.gameObject.SetActive(false);
        WeaponInventoryParent.gameObject.SetActive(false);
        ArmourInventoryParent.gameObject.SetActive(false);
        AccessoriesInventoryParent.gameObject.SetActive(false);

        selectedTab.SetActive(true);
    }

    public void SelectIcon(GearRuntime gearRuntime)
    {
        if (!SelectionUI.activeSelf)
            SelectionUI.SetActive(true);

        if (_selectedGear == gearRuntime) return;

        _gearSelectionUI.SelectGear(gearRuntime);
        _selectedGear = gearRuntime;

        // move all cards to disabled parent (preserve original order and normalize transforms)
        while (SelectedCardParent.childCount > 0)
        {
            Transform child = SelectedCardParent.GetChild(0);

            // Reparent without keeping world position so local positions are predictable
            child.SetParent(DisabledCardsParent, worldPositionStays: false);

            // Normalize RectTransform to avoid leftover offsets from previous usage
            var rect = child.GetComponent<RectTransform>();
            if (rect != null)
            {
                rect.localPosition = Vector3.zero;
                rect.localRotation = Quaternion.identity;
            }

            // Make sure container internal list is updated
            _cardContainer.RemoveCard(child.gameObject);
        }

        foreach (CardRuntime cardRuntime in gearRuntime.CardRuntimes)
        {
            GameObject cardObject = GetCard(); // try to reuse existing card if possible (object pooling)

            if (cardObject != null)
            {
                CardManager.SetCardDisplay(cardObject, cardRuntime);
                continue;
            }

            CardManager.CreateCard(cardRuntime, SelectedCardParent);
        }

        // Rebuild container state so positions/widths are correct immediately
        _cardContainer.InitCards();
    }

    private GameObject GetCard()
    {
        if (DisabledCardsParent.childCount == 0) return null;

        Transform child = DisabledCardsParent.GetChild(0);

        // Reparent without keeping world position so local positions are predictable
        child.SetParent(SelectedCardParent, worldPositionStays: false);

        // Normalize RectTransform to avoid visual jumps
        var rect = child.GetComponent<RectTransform>();
        if (rect != null)
        {
            rect.localPosition = Vector3.zero;
            rect.localRotation = Quaternion.identity;
        }

        return child.gameObject;
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
