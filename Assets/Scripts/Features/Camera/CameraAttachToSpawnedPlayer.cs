using Cinemachine;
using Core.Events;
using UnityEngine;

namespace Features.Camera
{
    public class CameraAttachToSpawnedPlayer : MonoBehaviour
    {
        private ICinemachineCamera _vCam;

        private void Awake()
        {
            _vCam = GetComponent<ICinemachineCamera>();
        }

        private void SetCameraTarget(Transform cameraTarget)
        {
            _vCam.Follow = cameraTarget;
            _vCam.VirtualCameraGameObject.transform.parent = cameraTarget;
        }

        private void OnEnable()
        {
            GameEvents.Player.OnPlayerSpawned += SetCameraTarget;
        }

        private void OnDisable()
        {
            GameEvents.Player.OnPlayerSpawned -= SetCameraTarget;
        }
    }
}
