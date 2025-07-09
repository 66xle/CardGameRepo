using Cinemachine;
using DG.Tweening;
using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class SwitchWeaponManager : MonoBehaviour
{
    [Header("References")]
    [MustBeAssigned] public EquipmentManager EquipmentManager;
    [MustBeAssigned] public CombatUIManager CombatUIManager;
    [MustBeAssigned] public WeaponDamageSettings WeaponRaritySettings;

    public WeaponData CurrentMainHand { get; set; }
    public WeaponData CurrentOffHand { get; set; }
    public List<WeaponData> CurrentEquippedWeapons { get; set; } = new();


    private List<CinemachineVirtualCamera> _cameraList = new();

    [Separator("Cameras")]

    public float TransitionSpeed = 1f;

    [MustBeAssigned] public CinemachineVirtualCamera EquipmentCam;
    [MustBeAssigned] public CinemachineVirtualCamera RightHipCam;
    [MustBeAssigned] public CinemachineVirtualCamera LeftHipCam;
    [MustBeAssigned] public CinemachineVirtualCamera BackCam;
    [MustBeAssigned] public CinemachineVirtualCamera LowerBackCam;
    [MustBeAssigned] public CinemachineVirtualCamera RightChestCam;
    [MustBeAssigned] public CinemachineVirtualCamera LeftChestCam;

    private int _cameraIndex;
    private bool _currSwitchCam;


    public void InitWeaponData()
    {
        // Reset Variables
        _currSwitchCam = false;
        CurrentEquippedWeapons.Clear();

        // Copy Data
        CurrentMainHand = CopyWeaponData(EquipmentManager.MainHand);

        if (IsOffhandEquipped())
            CurrentOffHand = CopyWeaponData(EquipmentManager.OffHand);

        if (IsHolstersEquipped())
        {
            foreach (WeaponData weaponData in EquipmentManager.GetEquippedWeapons())
            {
                CurrentEquippedWeapons.Add(CopyWeaponData(weaponData));
            }
        }
    }

    public void InitCameraList()
    {
        _cameraList.Clear();

        foreach (WeaponData data in CurrentEquippedWeapons)
        {
            Transform holsterTransform = data.HolsterSlot;
            CinemachineVirtualCamera camera = DetermineCamera(holsterTransform);

            if (camera != null)
            {
                _cameraList.Add(camera);
            }
        }
    }

    public List<WeaponData> GetWeaponList()
    {
        List<WeaponData> holsterWeapons = new List<WeaponData>();
        holsterWeapons.Add(CurrentMainHand);

        if (IsOffhandEquipped())
            holsterWeapons.Add(CurrentOffHand);

        holsterWeapons.AddRange(CurrentEquippedWeapons);

        return holsterWeapons;
    }

    private WeaponData CopyWeaponData(WeaponData data)
    {
        WeaponData newData = new WeaponData(data);

        // Scale weapon damage to rarity
        newData.WeaponAttack = WeaponRaritySettings.GetWeaponDamage(newData);

        newData.Guid = Guid.NewGuid().ToString();
        return newData;
    }

    public bool IsOffhandEquipped()
    {
        return EquipmentManager.OffHand != null ? true : false;
    }

    public bool IsHolstersEquipped()
    {
        return EquipmentManager.GetEquippedWeapons().Count > 0 ? true : false;
    }

    public void OpenSwitchWeaponUI()
    {
        _cameraIndex = 0;

        InitCameraList();

        SetEquipmentCameraPosition(_cameraList[_cameraIndex]);
        EquipmentCam.Priority = 50;

        CombatUIManager.SwitchWeaponUI.SetActive(true);
        CombatUIManager.CombatUI.SetActive(false);
    }

    public void NextWeapon()
    {
        if (_currSwitchCam)
            return;

        _cameraIndex++;

        if (_cameraIndex >= _cameraList.Count)
            _cameraIndex = 0;

        TransitionToNextWeapon(_cameraList[_cameraIndex]);
    }

    #region Camera

    private CinemachineVirtualCamera DetermineCamera(Transform holsterTransform)
    {

        if (holsterTransform.CompareTag("Left Hip"))
        {
            return LeftHipCam;
        }
        else if (holsterTransform.CompareTag("Right Hip"))
        {
            return RightHipCam;
        }
        else if (holsterTransform.CompareTag("Back"))
        {
            return BackCam;
        }
        else if (holsterTransform.CompareTag("Lower Back"))
        {
            return LowerBackCam;
        }
        else if (holsterTransform.CompareTag("Left Chest"))
        {
            return LeftChestCam;
        }
        else if (holsterTransform.CompareTag("Right Chest"))
        {
            return RightChestCam;
        }

        return null;
    }

    public void SetEquipmentCameraPosition(CinemachineVirtualCamera camera)
    {
        EquipmentCam.transform.position = camera.transform.position;
        EquipmentCam.transform.rotation = camera.transform.rotation;
        EquipmentCam.m_Lens = camera.m_Lens;

        CinemachineOrbitalTransposer camTransposer = camera.GetCinemachineComponent<CinemachineOrbitalTransposer>();
        CinemachineOrbitalTransposer equipTransposer = EquipmentCam.GetCinemachineComponent<CinemachineOrbitalTransposer>();

        equipTransposer.m_Heading = camTransposer.m_Heading;
        equipTransposer.m_FollowOffset = camTransposer.m_FollowOffset;



    }

    public void TransitionToNextWeapon(CinemachineVirtualCamera camera)
    {
        CombatUIManager.SwitchButton.interactable = false;

        DOVirtual.Float(EquipmentCam.m_Lens.Dutch, camera.m_Lens.Dutch, TransitionSpeed, v => EquipmentCam.m_Lens.Dutch = v);
        DOVirtual.Float(EquipmentCam.m_Lens.FieldOfView, camera.m_Lens.FieldOfView, TransitionSpeed, v => EquipmentCam.m_Lens.FieldOfView = v);

        EquipmentCam.transform.DORotate(camera.transform.rotation.eulerAngles, TransitionSpeed).OnComplete(() => CombatUIManager.SwitchButton.interactable = true);

        CinemachineOrbitalTransposer camTransposer = camera.GetCinemachineComponent<CinemachineOrbitalTransposer>();
        CinemachineOrbitalTransposer equipTransposer = EquipmentCam.GetCinemachineComponent<CinemachineOrbitalTransposer>();

        // Y offset
        DOVirtual.Float(equipTransposer.m_Heading.m_Bias, camTransposer.m_Heading.m_Bias, TransitionSpeed, v => equipTransposer.m_Heading.m_Bias = v);

        DOVirtual.Float(equipTransposer.m_FollowOffset.y, camTransposer.m_FollowOffset.y, TransitionSpeed, v => equipTransposer.m_FollowOffset.y = v);

        // Z offset
        DOVirtual.Float(equipTransposer.m_FollowOffset.z, -1, TransitionSpeed / 2, v => equipTransposer.m_FollowOffset.z = v).OnComplete(() =>
        {
            DOVirtual.Float(equipTransposer.m_FollowOffset.z, camTransposer.m_FollowOffset.z, TransitionSpeed / 2, v => equipTransposer.m_FollowOffset.z = v);
        });

    }

    #endregion
}
