using MyBox;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using Debug = UnityEngine.Debug;



public class EquipmentHolster : MonoBehaviour
{
    [Separator("Read Only")]
    [ReadOnly] public List<GameObject> EquippedWeaponObjects { get; private set; } = new List<GameObject>();

    [Separator("Transform")]
    public Transform RightHand;
    public Transform LeftHand;

    public Transform BackHolster;
    public Transform LowerBackHolster;

    public Transform RightHipHolster;
    public Transform LeftHipHolster;

    public Transform RightChestHolster;
    public Transform LeftChestHolster;


    [Separator("Holster Priority")]
    public List<Transform> SwordHolsterPriority;
    public List<Transform> TwoHandedHolsterPriority;
    public List<Transform> DaggerHolsterPriority;
    public List<Transform> ScytheHolsterPriority;

    

    public void EquipWeapon(GameObject weaponToEquip)
    {
        Vector3 localPos = weaponToEquip.transform.localPosition;
        Vector3 localRot = weaponToEquip.transform.localEulerAngles;

        weaponToEquip.transform.parent = RightHand;

        weaponToEquip.transform.localPosition = localPos;
        weaponToEquip.transform.localEulerAngles = localRot;
    }

    public void HolsterWeapon(GameObject weaponToEquip)
    {
        Weapon weaponScript = weaponToEquip.GetComponent<Weapon>();

        Vector3 localPos = weaponToEquip.transform.localPosition;
        Vector3 localRot = weaponToEquip.transform.localEulerAngles;

        weaponToEquip.transform.parent = weaponScript.HolsterParent;

        weaponToEquip.transform.localPosition = localPos;
        weaponToEquip.transform.localEulerAngles = localRot;
    }

    public void SetOffHand(WeaponData weaponData)
    {
        weaponData.HolsterSlot = LeftHand;
        CreateWeaponObject(weaponData.Prefab, LeftHand, weaponData);
    }

    public void SetHolsteredWeapons(List<WeaponData> weapons)
    {
        foreach (WeaponData data in weapons)
        {
            Weapon weaponScript = data.Prefab.GetComponent<Weapon>();
            
            if (weaponScript.WeaponType == WeaponType.Sword)
            {
                SetWeapon(SwordHolsterPriority, data, weaponScript);
            }
            if (weaponScript.WeaponType == WeaponType.Twohanded)
            {
                SetWeapon(TwoHandedHolsterPriority, data, weaponScript);
            }
            else if (weaponScript.WeaponType == WeaponType.Dagger)
            {
                SetWeapon(DaggerHolsterPriority, data, weaponScript);
            }
            else if (weaponScript.WeaponType == WeaponType.Scythe)
            {
                SetWeapon(ScytheHolsterPriority, data, weaponScript);
            }
        }
    }

    void SetWeapon(List<Transform> listTransformHolster, WeaponData data, Weapon weaponScript)
    {
        foreach (Transform holsterTransfrom in listTransformHolster)
        {
            if (GetChildLength(holsterTransfrom) == 0)
            {
                GameObject prefabToSpawn = DeterminePrefabOffset(holsterTransfrom, weaponScript);

                if (prefabToSpawn == null) 
                    prefabToSpawn = data.Prefab;

                data.HolsterSlot = holsterTransfrom;

                CreateWeaponObject(prefabToSpawn, holsterTransfrom, data);
                return;
            }
        }

        Debug.LogError("No Holster Slot Avaliable");
    }

    GameObject DeterminePrefabOffset(Transform holsterTransfrom, Weapon weaponScript)
    {
        if (holsterTransfrom.CompareTag("Back"))
        {
            if ((weaponScript.OffSetHolster & OffSetHolster.Back) == OffSetHolster.Back)
                return weaponScript.BackPrefab;
        }
        else if (holsterTransfrom.CompareTag("Lower Back"))
        {
            if ((weaponScript.OffSetHolster & OffSetHolster.LowerBack) == OffSetHolster.LowerBack)
                return weaponScript.LowerBackPrefab;
        }
        else if (holsterTransfrom.CompareTag("Left Hip"))
        {
            if ((weaponScript.OffSetHolster & OffSetHolster.LeftHip) == OffSetHolster.LeftHip)
                return weaponScript.LeftHipPrefab;
        }
        else if (holsterTransfrom.CompareTag("Right Hip"))
        {
            if ((weaponScript.OffSetHolster & OffSetHolster.RightHip) == OffSetHolster.RightHip)
                return weaponScript.RightHipPrefab;
        }
        else if (holsterTransfrom.CompareTag("Left Chest"))
        {
            if ((weaponScript.OffSetHolster & OffSetHolster.LeftChest) == OffSetHolster.LeftChest)
                return weaponScript.LeftChestPrefab;
        }
        else if (holsterTransfrom.CompareTag("Right Chest"))
        {
            if ((weaponScript.OffSetHolster & OffSetHolster.RightChest) == OffSetHolster.RightChest)
                return weaponScript.RightChestPrefab;
        }

        return null;
    }

    private void CreateWeaponObject(GameObject prefab, Transform parent, WeaponData data)
    {
        GameObject newWeapon = Instantiate(prefab, parent);
        EquippedWeaponObjects.Add(newWeapon);

        Weapon weaponScript = newWeapon.GetComponent<Weapon>();
        weaponScript.Guid = data.Guid;
        weaponScript.HolsterParent = parent;
    }

    int GetChildLength(Transform parent)
    {
        return parent.childCount;
    }
}
