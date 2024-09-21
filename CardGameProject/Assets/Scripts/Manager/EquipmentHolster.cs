using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentHolster : MonoBehaviour
{
    public Transform rightHand;
    public Transform leftHand;
    public Transform lowerBackHolster;
    public Transform rightHipHolster;
    public Transform leftHipHolster;
    public Transform backHolster;
    public Transform leftChestHolster;
    public Transform rightChestHolster;

    public void SetMainHand(GameObject weapon)
    {
        Instantiate(weapon, rightHand);
    }

    public void SetHolsteredWeapons(List<GameObject> weapons)
    {
        foreach (GameObject weapon in weapons)
        {
            if (weapon.CompareTag("Sword"))
            {
                SetSword(weapon);
            }
        }
    }

    void SetSword(GameObject weapon)
    {
        if (GetChildLength(leftHipHolster) == 0)
        {
            Instantiate(weapon, leftHipHolster.transform);
        } 
        else if (GetChildLength(backHolster) == 0)
        {
            Instantiate(weapon, backHolster.transform);
        }
        else
        {
            Debug.LogError("No Holster Slot avaliable!");
        }
    }

    int GetChildLength(Transform parent)
    {
        return parent.childCount;
    }
}
