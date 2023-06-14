using System;
using System.Collections;
using System.Collections.Generic;
using BitStrap;
using Elad.Scripts;
using UnityEngine;
using UnityEngine.Playables;
using Logger = Nemesh.Logger;

public class TimeLine : MonoBehaviour
{
    private bool _started;
    private PlayableDirector _playableDirector;

    private void Awake()
    {
        _playableDirector = GetComponent<PlayableDirector>();
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(TagStrings.playerTag))
        {
            StartCutScene();
        }
    }

    [Button]
    public void StartCutScene()
    {
        if (!_started)
        {
            _started = true;
            _playableDirector.Play();
            var zoomCam = PlayerStatus.CurrentVirtualCamara.GetComponent<ZoomCamera>();
            zoomCam.StartZoom();
        }
    }
}