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
    [ReadOnly]
    public List<GameObject> equippedWeaponObjects = new List<GameObject>();

    [Separator("Transform")]
    public Transform rightHand;
    public Transform leftHand;

    public Transform backHolster;
    public Transform lowerBackHolster;

    public Transform rightHipHolster;
    public Transform leftHipHolster;

    public Transform rightChestHolster;
    public Transform leftChestHolster;


    [Separator("Holster Priority")]
    public List<Transform> swordHolsterPriority;
    public List<Transform> twoHandedHolsterPriority;
    public List<Transform> daggerHolsterPriority;
    public List<Transform> scytheHolsterPriority;

    

    public void EquipWeapon(GameObject weaponToEquip)
    {
        Vector3 localPos = weaponToEquip.transform.localPosition;
        Vector3 localRot = weaponToEquip.transform.localEulerAngles;

        weaponToEquip.transform.parent = rightHand;

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
        weaponData.holsterSlot = leftHand;
        CreateWeaponObject(weaponData.prefab, leftHand, weaponData);
    }

    public void SetHolsteredWeapons(List<WeaponData> weapons)
    {
        foreach (WeaponData data in weapons)
        {
            Weapon weaponScript = data.prefab.GetComponent<Weapon>();
            
            if (weaponScript.weaponType == WeaponType.Sword)
            {
                SetWeapon(swordHolsterPriority, data, weaponScript);
            }
            if (weaponScript.weaponType == WeaponType.Twohanded)
            {
                SetWeapon(twoHandedHolsterPriority, data, weaponScript);
            }
            else if (weaponScript.weaponType == WeaponType.Dagger)
            {
                SetWeapon(daggerHolsterPriority, data, weaponScript);
            }
            else if (weaponScript.weaponType == WeaponType.Scythe)
            {
                SetWeapon(scytheHolsterPriority, data, weaponScript);
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
                    prefabToSpawn = data.prefab;

                data.holsterSlot = holsterTransfrom;

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
            if ((weaponScript.offSetHolster & OffSetHolster.Back) == OffSetHolster.Back)
                return weaponScript.backPrefab;
        }
        else if (holsterTransfrom.CompareTag("Lower Back"))
        {
            if ((weaponScript.offSetHolster & OffSetHolster.LowerBack) == OffSetHolster.LowerBack)
                return weaponScript.lowerBackPrefab;
        }
        else if (holsterTransfrom.CompareTag("Left Hip"))
        {
            if ((weaponScript.offSetHolster & OffSetHolster.LeftHip) == OffSetHolster.LeftHip)
                return weaponScript.leftHipPrefab;
        }
        else if (holsterTransfrom.CompareTag("Right Hip"))
        {
            if ((weaponScript.offSetHolster & OffSetHolster.RightHip) == OffSetHolster.RightHip)
                return weaponScript.rightHipPrefab;
        }
        else if (holsterTransfrom.CompareTag("Left Chest"))
        {
            if ((weaponScript.offSetHolster & OffSetHolster.LeftChest) == OffSetHolster.LeftChest)
                return weaponScript.leftChestPrefab;
        }
        else if (holsterTransfrom.CompareTag("Right Chest"))
        {
            if ((weaponScript.offSetHolster & OffSetHolster.RightChest) == OffSetHolster.RightChest)
                return weaponScript.rightChestPrefab;
        }

        return null;
    }

    private void CreateWeaponObject(GameObject prefab, Transform parent, WeaponData data)
    {
        GameObject newWeapon = Instantiate(prefab, parent);
        equippedWeaponObjects.Add(newWeapon);

        Weapon weaponScript = newWeapon.GetComponent<Weapon>();
        weaponScript.Guid = data.guid;
        weaponScript.HolsterParent = parent;
    }

    int GetChildLength(Transform parent)
    {
        return parent.childCount;
    }
}
