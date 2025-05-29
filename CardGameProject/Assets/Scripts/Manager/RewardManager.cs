using MyBox;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class RewardManager : MonoBehaviour
{
    [MustBeAssigned] [SerializeField] UIManager UIManager;
    [MustBeAssigned] [SerializeField] EquipmentManager EquipmentManager;
    [MustBeAssigned] [SerializeField] Transform DisplayRewardsUI;
    [MustBeAssigned] [SerializeField] GameObject IconPrefab;
    [SerializeField] List<WeaponData> PoolOfGear;

    private List<WeaponData> listOfWeaponReward = new();

    public void ClaimGear()
    {
        EquipmentManager.AddWeapon(listOfWeaponReward[0]);

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

        EquipmentManager.SaveWeapons();

        GameObject icon = Instantiate(IconPrefab, DisplayRewardsUI);

        RawImage image = icon.GetComponent<RawImage>();
        image.texture = weaponData.IconTexture;
    }
}
