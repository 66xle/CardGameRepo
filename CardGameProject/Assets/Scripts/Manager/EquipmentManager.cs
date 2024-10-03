using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEditorInternal;
using DG.Tweening;
using System;

public class EquipmentManager : MonoBehaviour
{
    public WeaponData mainHand;
    public WeaponData offHand;

    [Separator]

    public List<WeaponData> equippedWeapons;

    [Separator("Cameras")]

    public CinemachineVirtualCamera equipmentCam;
    public CinemachineVirtualCamera lowerBackCam;
    public CinemachineVirtualCamera rightHipCam;


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
        equipmentCam.transform.DORotate(camera.transform.rotation.eulerAngles, 1f);

        CinemachineOrbitalTransposer camTransposer = camera.GetCinemachineComponent<CinemachineOrbitalTransposer>();
        CinemachineOrbitalTransposer equipTransposer = equipmentCam.GetCinemachineComponent<CinemachineOrbitalTransposer>();

        DOVirtual.Float(equipTransposer.m_Heading.m_Bias, camTransposer.m_Heading.m_Bias, 1f, v => equipTransposer.m_Heading.m_Bias = v);

        DOVirtual.Float(equipTransposer.m_FollowOffset.y, camTransposer.m_FollowOffset.y, 1f, v => equipTransposer.m_FollowOffset.y = v);


        DOVirtual.Float(equipTransposer.m_FollowOffset.z, -1, 0.5f, v => equipTransposer.m_FollowOffset.z = v).OnComplete(() =>
        {
            DOVirtual.Float(equipTransposer.m_FollowOffset.z, camTransposer.m_FollowOffset.z, 0.5f, v => equipTransposer.m_FollowOffset.z = v);
        });

    }
}
