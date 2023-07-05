using Cinemachine;
using Elad.Scripts;
using Managers;
using UnityEngine;

public class RoomTransition : MonoBehaviour
{
    [SerializeField] private GameObject virtualCamPrefab;

    private GameObject _player;
    private GameObject virtualCam;

    private void Start()
    {
        _player = PlayerStatus.Player;
        SetCamera();
    }

    private void SetCamera()
    {
        virtualCam = Instantiate(virtualCamPrefab, transform);

        CinemachineVirtualCamera cam = virtualCam.GetComponent<CinemachineVirtualCamera>();
        CinemachineConfiner confiner = virtualCam.GetComponent<CinemachineConfiner>();
        if (cam != null && confiner != null)
        {
            cam.Follow = _player.transform;
            confiner.m_BoundingShape2D = GetComponent<PolygonCollider2D>();
        }
        else
        {
            Debug.Log("Room Couldn't link player or collider to camera!");
        }

        virtualCam.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !other.isTrigger)
        {
            virtualCam.SetActive(true);
            CameraManager.CrouchCameraController = virtualCam.GetComponent<CrouchCamera>();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !other.isTrigger)
        {
            virtualCam.SetActive(false);
            AudioManager.instance.PlayOneShot(FMODEvents.instance.camMovement, transform.position);

        }
    }
}