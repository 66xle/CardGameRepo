using MyBox;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using Debug = UnityEngine.Debug;



public class EquipmentHolster : MonoBehaviour
{
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

    public void SetMainHand(GameObject weapon)
    {
        Instantiate(weapon, rightHand);
    }

    public void SetOffHand(GameObject offHand)
    {
        Instantiate(offHand, leftHand);
    }

    public void SetHolsteredWeapons(List<GameObject> weapons)
    {
        foreach (GameObject weaponPrefab in weapons)
        {
            Weapon weaponScript = weaponPrefab.GetComponent<Weapon>();
            
            if (weaponScript.weaponType == WeaponType.Sword)
            {
                SetWeapon(swordHolsterPriority, weaponPrefab, weaponScript);
            }
            if (weaponScript.weaponType == WeaponType.Twohanded)
            {
                SetWeapon(twoHandedHolsterPriority, weaponPrefab, weaponScript);
            }
            else if (weaponScript.weaponType == WeaponType.Dagger)
            {
                SetWeapon(daggerHolsterPriority, weaponPrefab, weaponScript);
            }
            else if (weaponScript.weaponType == WeaponType.Scythe)
            {
                SetWeapon(scytheHolsterPriority, weaponPrefab, weaponScript);
            }
        }
    }

    void SetWeapon(List<Transform> listTransformHolster, GameObject prefab, Weapon weaponScript)
    {
        foreach (Transform holsterTransfrom in listTransformHolster)
        {
            if (GetChildLength(holsterTransfrom) == 0)
            {
                GameObject prefabToSpawn = DeterminePrefab(holsterTransfrom, weaponScript);

                if (prefabToSpawn == null) 
                    prefabToSpawn = prefab;

                Instantiate(prefabToSpawn, holsterTransfrom);
                return;
            }
        }
    }

    GameObject DeterminePrefab(Transform holsterTransfrom, Weapon weaponScript)
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


    int GetChildLength(Transform parent)
    {
        return parent.childCount;
    }
}
