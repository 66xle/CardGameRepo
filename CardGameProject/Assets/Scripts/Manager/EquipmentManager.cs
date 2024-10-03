using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEditorInternal;
using DG.Tweening;
using System;
using UnityEngine.UI;

public class EquipmentManager : MonoBehaviour
{
    public WeaponData mainHand;
    public WeaponData offHand;

    public Button switchButton;

    [HideInInspector] public WeaponData currentMainHand;
    [HideInInspector] public WeaponData currentOffHand;

    [Separator]

    public List<WeaponData> equippedWeapons;
    [HideInInspector] public List<WeaponData> currentEquippedWeapons;


    [HideInInspector] public List<CinemachineVirtualCamera> cameraList;

    [Separator("Cameras")]

    public CinemachineVirtualCamera equipmentCam;
    public CinemachineVirtualCamera rightHipCam;
    public CinemachineVirtualCamera leftHipCam;
    public CinemachineVirtualCamera backCam;
    public CinemachineVirtualCamera lowerBackCam;
    public CinemachineVirtualCamera rightChestCam;
    public CinemachineVirtualCamera leftChestCam;


    [HideInInspector] public int cameraIndex;
    [HideInInspector] public bool currSwitchCam;


    public void InitWeaponData()
    {
        currSwitchCam = false;

        currentEquippedWeapons.Clear();

        currentMainHand = CopyWeaponData(mainHand);

        if (offHand != null)
            currentOffHand = CopyWeaponData(offHand);

        foreach (WeaponData weaponData in equippedWeapons)
        {
            currentEquippedWeapons.Add(CopyWeaponData(weaponData));
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

        CinemachineOrbitalTransposer camTransposer = camera.GetCinemachineComponent<CinemachineOrbitalTransposer>();
        CinemachineOrbitalTransposer equipTransposer = equipmentCam.GetCinemachineComponent<CinemachineOrbitalTransposer>();

        equipTransposer.m_Heading = camTransposer.m_Heading;
        equipTransposer.m_FollowOffset = camTransposer.m_FollowOffset;

    }

    public void TransitionToNextWeapon(CinemachineVirtualCamera camera)
    {

        switchButton.interactable = false;

        equipmentCam.transform.DORotate(camera.transform.rotation.eulerAngles, 1f).OnComplete(() => switchButton.interactable = true);

        CinemachineOrbitalTransposer camTransposer = camera.GetCinemachineComponent<CinemachineOrbitalTransposer>();
        CinemachineOrbitalTransposer equipTransposer = equipmentCam.GetCinemachineComponent<CinemachineOrbitalTransposer>();

        // Y offset
        DOVirtual.Float(equipTransposer.m_Heading.m_Bias, camTransposer.m_Heading.m_Bias, 1f, v => equipTransposer.m_Heading.m_Bias = v);

        DOVirtual.Float(equipTransposer.m_FollowOffset.y, camTransposer.m_FollowOffset.y, 1f, v => equipTransposer.m_FollowOffset.y = v);

        // Z offset
        DOVirtual.Float(equipTransposer.m_FollowOffset.z, -1, 0.5f, v => equipTransposer.m_FollowOffset.z = v).OnComplete(() =>
        {
            DOVirtual.Float(equipTransposer.m_FollowOffset.z, camTransposer.m_FollowOffset.z, 0.5f, v => equipTransposer.m_FollowOffset.z = v);
        });

    }
}
