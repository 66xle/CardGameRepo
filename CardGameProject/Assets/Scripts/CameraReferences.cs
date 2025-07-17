using Cinemachine;
using MyBox;
using System.Collections.Generic;
using UnityEngine;

public class CameraReferences : MonoBehaviour
{
    [Header("Lists")]
    [SerializeField] List<CinemachineVirtualCamera> ListIdleCamera;
    [SerializeField] List<CinemachineVirtualCamera> ListDefaultCamera;
    [SerializeField] List<CinemachineVirtualCamera> ListFollowCamera;
    [SerializeField] List<CinemachineVirtualCamera> ListFollowBackCamera;
    [SerializeField] List<CinemachineVirtualCamera> ListAttackCamera;

    [SerializeField] List<CinemachineVirtualCamera> FollowTarget;

    [MustBeAssigned] [SerializeField] CinemachineVirtualCamera EquipmentCam;
    [MustBeAssigned][SerializeField] CinemachineVirtualCamera RightHipCam;
    [MustBeAssigned][SerializeField] CinemachineVirtualCamera LeftHipCam;
    [MustBeAssigned][SerializeField] CinemachineVirtualCamera BackCam;
    [MustBeAssigned][SerializeField] CinemachineVirtualCamera LowerBackCam;
    [MustBeAssigned][SerializeField] CinemachineVirtualCamera RightChestCam;
    [MustBeAssigned][SerializeField] CinemachineVirtualCamera LeftChestCam;

    public void SetReferences(CameraManager cm, SwitchWeaponManager swm)
    {
        cm.ListIdleCamera.AddRange(ListIdleCamera);
        cm.ListDefaultCamera.AddRange(ListDefaultCamera);
        cm.ListFollowCamera.AddRange(ListFollowCamera);
        cm.ListFollowBackCamera.AddRange(ListFollowBackCamera);
        cm.ListAttackCamera.AddRange(ListAttackCamera);
        cm.FollowTarget.AddRange(FollowTarget);

        swm.EquipmentCam = EquipmentCam;
        swm.RightHipCam = RightHipCam;
        swm.LeftHipCam = LeftHipCam;
        swm.BackCam = BackCam;
        swm.LowerBackCam = LowerBackCam;
        swm.RightChestCam = RightChestCam;
        swm.LeftChestCam = LeftChestCam;
    }

    
}
