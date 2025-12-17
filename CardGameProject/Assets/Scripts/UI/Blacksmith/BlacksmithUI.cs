using MyBox;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BlacksmithUI : MonoBehaviour
{
    [MustBeAssigned] [SerializeField] GameObject GearIconPrefab;
    [MustBeAssigned] [SerializeField] Transform InventoryParent;
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

        foreach (GearData data in weapons)
        {
            GameObject iconObj = Instantiate(GearIconPrefab, InventoryParent);
            GearIconUI iconUI = iconObj.GetComponent<GearIconUI>();
            iconUI.SetData(data, _onClickSelectIcon);
        }
    }

    public void SelectIcon(GearData data)
    {
        GearSelectionUI.gameObject.SetActive(true);
        GearSelectionUI.SelectGear(data);
        _selectedGear = data;
    }
}
