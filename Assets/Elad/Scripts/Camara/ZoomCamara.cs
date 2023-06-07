using System.Collections;
using BitStrap;
using UnityEngine;
using Cinemachine;
using Elad.Scripts;
using Logger = Nemesh.Logger;

public class ZoomCamera : MonoBehaviour
{
    private CinemachineVirtualCamera  _cam;
    private CinemachineTransposer _transpose;
    
    private float startDistance;
    [SerializeField] private float wantedDistance;
    [SerializeField] private float zoomSpeed = 0.2f;


    
    private void Awake()
    {
        _cam = GetComponent<CinemachineVirtualCamera>();
        
        _cam.LookAt = PlayerStatus.player.transform;
        _cam.Follow = PlayerStatus.player.transform;
        
        startDistance = _cam.m_Lens.OrthographicSize;
    }

    
    // Function to zoom the camera to a specified distance over time
    public void ZoomToDistance()
    {
        StopAllCoroutines();
        StartCoroutine(ZoomCoroutine());
    }

    private IEnumerator ZoomCoroutine()
    {
        float currentDistance = _cam.m_Lens.OrthographicSize;
        float t = 0f;

        while (Mathf.Abs(currentDistance - wantedDistance) > 0.01f)
        {
            t += Time.deltaTime * zoomSpeed;
            currentDistance = Mathf.Lerp(currentDistance, wantedDistance, t);
            _cam.m_Lens.OrthographicSize = currentDistance;
            yield return null;
            
        }
        Logger.Log("Got to the final zoom distance");
    }
    
    [Button]
    public void StartZoom()
    {
        ZoomToDistance();
    }
}