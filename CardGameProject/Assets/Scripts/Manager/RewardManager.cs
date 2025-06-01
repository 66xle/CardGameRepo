using MyBox;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class RewardManager : MonoBehaviour
{
    [MustBeAssigned] [SerializeField] UIManager UIManager;
    [MustBeAssigned] [SerializeField] EquipmentManager EquipmentManager;

    [MustBeAssigned] [SerializeField] GameObject GearOverlay;
    [MustBeAssigned] [SerializeField] Transform DisplayRewardsUI;
    [MustBeAssigned] [SerializeField] GameObject IconPrefab;
    [MustBeAssigned] [SerializeField] Camera RenderCamera;



    [SerializeField] List<WeaponData> PoolOfGear;

    private List<WeaponData> listOfWeaponReward = new();
    private GameObject currentObjectInOverlay;

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

        GearOverlay.SetActive(true);
    }

    public void CloseGearOverlay()
    {
        RenderCamera.gameObject.SetActive(false);
        Destroy(currentObjectInOverlay);

        GearOverlay.SetActive(false);
    }
}
