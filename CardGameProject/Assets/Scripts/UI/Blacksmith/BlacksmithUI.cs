using MyBox;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class BlacksmithUI : MonoBehaviour
{
    [MustBeAssigned] [SerializeField] GameObject GearIconPrefab;
    [MustBeAssigned] [SerializeField] Transform AllInventoryParent;
    [MustBeAssigned] [SerializeField] Transform WeaponInventoryParent;
    [MustBeAssigned] [SerializeField] Transform ArmourInventoryParent;
    [MustBeAssigned] [SerializeField] Transform AccessoriesInventoryParent;
    [MustBeAssigned] [SerializeField] GearSelectionUI GearSelectionUI;

    Action<GearData> _onClickSelectIcon;
    GearData _selectedGear;

    [MustBeAssigned] [SerializeField] EquipmentManager equipmentManager; // temp for testing

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        equipmentManager.SaveGear(); // temp for testing

        _onClickSelectIcon += SelectIcon;
        LoadInventory();
    }

    void LoadInventory()
    {
        List<GearData> weapons = new List<GearData>(GameManager.Instance.EquippedWeapons);
        weapons.Add(GameManager.Instance.MainHand);

        List<GearData> armours = new List<GearData>(GameManager.Instance.EquippedArmour);


        foreach (GearData data in weapons)
        {
            CreateGearIcon(data, AllInventoryParent);
            CreateGearIcon(data, WeaponInventoryParent);
        }

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

    public void SelectIcon(GearData data)
    {
        GearSelectionUI.gameObject.SetActive(true);
        GearSelectionUI.SelectGear(data);
        _selectedGear = data;
    }
}
