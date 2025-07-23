using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using MyBox;
using UnityEngine.Playables;

public class CameraManager : MonoBehaviour
{
    public int ActivePriority = 30;
    [MustBeAssigned] [SerializeField] Transform CameraDummy;
    [MustBeAssigned] [SerializeField] Transform VictimDummy;
    [MustBeAssigned] [SerializeField] PlayableDirector TimelineDirector;

    [Header("Lists")]
    [SerializeField] List<CinemachineVirtualCamera> ListIdleCamera;
    [SerializeField] List<CinemachineVirtualCamera> ListDefaultCamera;
    [SerializeField] List<CinemachineVirtualCamera> ListFollowCamera;
    [SerializeField] List<CinemachineVirtualCamera> ListFollowBackCamera;
    [SerializeField] List<CinemachineVirtualCamera> ListAttackCamera;

    [SerializeField] List<CinemachineVirtualCamera> FollowTarget;


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
        if (_activeCamera == chosenCamera)
            return;

        if (_activeCamera != null)
            _activeCamera.Priority = 0;

        _activeCamera = chosenCamera;
        _activeCamera.Priority = ActivePriority;
    }

    public void SetTimeline(PlayableAsset timeline)
    {
        TimelineDirector.playableAsset = timeline;
    }

    public void ToggleDirector(bool toggle)
    {
        if (toggle)
        {
            TimelineDirector.Play();
            return;
        }

        TimelineDirector.Stop();
    }

    private void SetFollowTarget(Transform player)
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

    #region Dummy

    public void SetDummy(Transform avatarTransform)
    {
        CameraDummy.position = avatarTransform.position;
        CameraDummy.rotation = avatarTransform.rotation;

        SetFollowTarget(avatarTransform);
    }

    public void SetVictimDummy(Transform victim, Transform attacker)
    {
        VictimDummy.position = victim.position;

        Vector3 direction = attacker.position - victim.position;
        direction.y = 0;

        VictimDummy.rotation = Quaternion.LookRotation(direction);
    }

    #endregion

    #region States

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

    #endregion
}
