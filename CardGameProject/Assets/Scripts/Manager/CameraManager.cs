using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using MyBox;

public class CameraManager : MonoBehaviour
{
    public float ActivePriority = 30;


    [Header("Lists")]
    public List<CinemachineVirtualCamera> ListIdleCamera;
    public List<CinemachineVirtualCamera> ListDefaultCamera;
    public List<CinemachineVirtualCamera> ListFollowCamera;
    public List<CinemachineVirtualCamera> ListFollowBackCamera;
    public List<CinemachineVirtualCamera> ListAttackCamera;

    public List<CinemachineVirtualCamera> FollowTarget;


    [HideInInspector] public CinemachineVirtualCamera EquipmentCam;
    [HideInInspector] public CinemachineVirtualCamera RightHipCam;
    [HideInInspector] public CinemachineVirtualCamera LeftHipCam;
    [HideInInspector] public CinemachineVirtualCamera BackCam;
    [HideInInspector] public CinemachineVirtualCamera LowerBackCam;
    [HideInInspector] public CinemachineVirtualCamera RightChestCam;
    [HideInInspector] public CinemachineVirtualCamera LeftChestCam;

    private CinemachineVirtualCamera _activeCamera;


    public void ActivateCamera(CinemachineVirtualCamera chosenCamera)
    {
        if (_activeCamera != null)
            _activeCamera.Priority = 0;

        _activeCamera = chosenCamera;
        _activeCamera.Priority = 30;
    }

    public void SetFollowTarget(Transform player)
    {
        foreach (CinemachineVirtualCamera cam in FollowTarget)
        {
            cam.Follow = player;
        }
    }

    public CinemachineVirtualCamera GetCamera(List<CinemachineVirtualCamera> listOfCamera)
    {
        int index = Random.Range(0, listOfCamera.Count);
        return listOfCamera[index];
    }

    public void DefaultState()
    {
        CinemachineVirtualCamera cam = GetCamera(ListDefaultCamera);
        ActivateCamera(cam);
    }

    public void FollowState()
    {
        CinemachineVirtualCamera cam = GetCamera(ListFollowCamera);
        ActivateCamera(cam);
    }
    
    public void FollowBackState()
    {
        CinemachineVirtualCamera cam = GetCamera(ListFollowBackCamera);
        ActivateCamera(cam);
    }

    public void AttackState()
    {
        CinemachineVirtualCamera cam = GetCamera(ListAttackCamera);
        ActivateCamera(cam);
    }
}
