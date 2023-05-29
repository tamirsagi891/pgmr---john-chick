using System;
using System.Collections;
using System.Collections.Generic;
using Elad.Events;
using UnityEngine;

public class Platform : MonoBehaviour
{

    [SerializeField] private float noSurfaceTime = 0.5f;
    [SerializeField] private float noSurfaceTimer;

    private bool noSurface;
    private PlatformEffector2D _platformEffector2D;


    private void Awake()
    {
        _platformEffector2D = GetComponent<PlatformEffector2D>();
    }

    private void OnEnable()
    {
        characterEvents.playerCrouchAndJumpOnPlatform.AddListener(playerCrouchAndJump);
    }

    private void OnDisable()
    {
        characterEvents.playerCrouchAndJumpOnPlatform.RemoveListener(playerCrouchAndJump);
    }

    private void Update()
    {
        if (noSurface)
        {
            noSurfaceTimer -= Time.deltaTime;
            if (noSurfaceTimer <= 0)
            {
                _platformEffector2D.surfaceArc = 180;
                noSurface = false;
            }
        }
    }


    private void playerCrouchAndJump(bool state)
    {
        _platformEffector2D.surfaceArc = 0;
        noSurfaceTimer = noSurfaceTime;
        noSurface = true;
    }

    
}