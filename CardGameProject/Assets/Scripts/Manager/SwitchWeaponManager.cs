using Cinemachine;
using DG.Tweening;
using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SwitchWeaponManager : MonoBehaviour
{
    [Header("References")]
    public EquipmentManager equipmentManager;
    public CombatUIManager combatUIManager;

    [HideInInspector] public WeaponData currentMainHand;
    [HideInInspector] public WeaponData currentOffHand;
    [HideInInspector] public List<WeaponData> currentEquippedWeapons;
    private List<CinemachineVirtualCamera> cameraList = new List<CinemachineVirtualCamera>();

    [Separator("Cameras")]

    public float transitionSpeed = 1f;

    public CinemachineVirtualCamera equipmentCam;
    public CinemachineVirtualCamera rightHipCam;
    public CinemachineVirtualCamera leftHipCam;
    public CinemachineVirtualCamera backCam;
    public CinemachineVirtualCamera lowerBackCam;
    public CinemachineVirtualCamera rightChestCam;
    public CinemachineVirtualCamera leftChestCam;

    private int cameraIndex;
    private bool currSwitchCam;


    public void InitWeaponData()
    {
        // Reset Variables
        currSwitchCam = false;
        currentEquippedWeapons.Clear();

        // Copy Data
        currentMainHand = CopyWeaponData(equipmentManager.mainHand);

        if (IsOffhandEquipped())
            currentOffHand = CopyWeaponData(equipmentManager.offHand);

        if (IsHolstersEquipped())
        {
            foreach (WeaponData weaponData in equipmentManager.equippedWeapons)
            {
                currentEquippedWeapons.Add(CopyWeaponData(weaponData));
            }
        }
    }

    public void InitCameraList()
    {
        cameraList.Clear();

        foreach (WeaponData data in currentEquippedWeapons)
        {
            Transform holsterTransform = data.holsterSlot;
            CinemachineVirtualCamera camera = DetermineCamera(holsterTransform);

            if (camera != null)
            {
                cameraList.Add(camera);
            }
        }
    }

    private WeaponData CopyWeaponData(WeaponData data)
    {
        WeaponData newData = new WeaponData();

        newData.weaponName = data.weaponName;
        newData.description = data.description;
        newData.cards = data.cards;
        newData.prefab = data.prefab;


        return newData;
    }

    public bool IsOffhandEquipped()
    {
        return equipmentManager.offHand != null ? true : false;
    }

    public bool IsHolstersEquipped()
    {
        return equipmentManager.equippedWeapons.Count > 0 ? true : false;
    }

    public void OpenSwitchWeaponUI()
    {
        cameraIndex = 0;

        InitCameraList();

        SetEquipmentCameraPosition(cameraList[cameraIndex]);
        equipmentCam.Priority = 50;

        combatUIManager.switchWeaponUI.SetActive(true);
        combatUIManager.combatUI.SetActive(false);
    }

    public void NextWeapon()
    {
        if (currSwitchCam)
            return;

        cameraIndex++;

        if (cameraIndex >= cameraList.Count)
            cameraIndex = 0;

        TransitionToNextWeapon(cameraList[cameraIndex]);
    }

    #region Camera

    private CinemachineVirtualCamera DetermineCamera(Transform holsterTransform)
    {

        if (holsterTransform.CompareTag("Left Hip"))
        {
            return leftHipCam;
        }
        else if (holsterTransform.CompareTag("Right Hip"))
        {
            return rightHipCam;
        }
        else if (holsterTransform.CompareTag("Back"))
        {
            return backCam;
        }
        else if (holsterTransform.CompareTag("Lower Back"))
        {
            return lowerBackCam;
        }
        else if (holsterTransform.CompareTag("Left Chest"))
        {
            return leftChestCam;
        }
        else if (holsterTransform.CompareTag("Right Chest"))
        {
            return rightChestCam;
        }

        return null;
    }

    public void SetEquipmentCameraPosition(CinemachineVirtualCamera camera)
    {
        equipmentCam.transform.position = camera.transform.position;
        equipmentCam.transform.rotation = camera.transform.rotation;
        equipmentCam.m_Lens = camera.m_Lens;

        CinemachineOrbitalTransposer camTransposer = camera.GetCinemachineComponent<CinemachineOrbitalTransposer>();
        CinemachineOrbitalTransposer equipTransposer = equipmentCam.GetCinemachineComponent<CinemachineOrbitalTransposer>();

        equipTransposer.m_Heading = camTransposer.m_Heading;
        equipTransposer.m_FollowOffset = camTransposer.m_FollowOffset;



    }

    public void TransitionToNextWeapon(CinemachineVirtualCamera camera)
    {
        combatUIManager.switchButton.interactable = false;

        DOVirtual.Float(equipmentCam.m_Lens.Dutch, camera.m_Lens.Dutch, transitionSpeed, v => equipmentCam.m_Lens.Dutch = v);
        DOVirtual.Float(equipmentCam.m_Lens.FieldOfView, camera.m_Lens.FieldOfView, transitionSpeed, v => equipmentCam.m_Lens.FieldOfView = v);

        equipmentCam.transform.DORotate(camera.transform.rotation.eulerAngles, transitionSpeed).OnComplete(() => combatUIManager.switchButton.interactable = true);

        CinemachineOrbitalTransposer camTransposer = camera.GetCinemachineComponent<CinemachineOrbitalTransposer>();
        CinemachineOrbitalTransposer equipTransposer = equipmentCam.GetCinemachineComponent<CinemachineOrbitalTransposer>();

        // Y offset
        DOVirtual.Float(equipTransposer.m_Heading.m_Bias, camTransposer.m_Heading.m_Bias, transitionSpeed, v => equipTransposer.m_Heading.m_Bias = v);

        DOVirtual.Float(equipTransposer.m_FollowOffset.y, camTransposer.m_FollowOffset.y, transitionSpeed, v => equipTransposer.m_FollowOffset.y = v);

        // Z offset
        DOVirtual.Float(equipTransposer.m_FollowOffset.z, -1, transitionSpeed / 2, v => equipTransposer.m_FollowOffset.z = v).OnComplete(() =>
        {
            DOVirtual.Float(equipTransposer.m_FollowOffset.z, camTransposer.m_FollowOffset.z, transitionSpeed / 2, v => equipTransposer.m_FollowOffset.z = v);
        });

    }

    #endregion
}
