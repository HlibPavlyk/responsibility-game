using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CameraAttachToSpawnedPlayer : MonoBehaviour
{
    ICinemachineCamera vCam;

    UnityAction<Transform> setCameraTargetAction;

    private void Awake()
    {
        vCam = GetComponent<ICinemachineCamera>();

        setCameraTargetAction = new UnityAction<Transform>(SetCameraTarget);
    }

    private void SetCameraTarget(Transform cameraTarget)
    {
        vCam.Follow = cameraTarget;
        vCam.VirtualCameraGameObject.transform.parent = cameraTarget;
    }

    private void OnEnable()
    {
        PlayerEvents.onPlayerSpawned += setCameraTargetAction;
    }

    private void OnDisable()
    {
        PlayerEvents.onPlayerSpawned -= setCameraTargetAction;
    }
}
