using Cinemachine;

namespace Managers
{
    public static class CameraManager
    {
        public static CinemachineVirtualCamera CurrentVirtualCamara => CrouchCameraController.VirtualCamera;
        public static CrouchCamera CrouchCameraController { get; set; }
    }
}