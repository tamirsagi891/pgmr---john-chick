using System;
using System.Collections;
using System.Collections.Generic;
using Elad.Events;
using Elad.Scripts;
using UnityEngine;
using Logger = Nemesh.Logger;

public class Platform : MonoBehaviour
{

    // [SerializeField] private float noSurfaceTime = 0.5f;
    // private float noSurfaceTimer;

    private bool _noSurface;
    private PlatformEffector2D _platformEffector2D;
    private bool _isMovingThrowPlatform;

    public bool IsMovingThrowPlatform
    {
        get => _isMovingThrowPlatform;
        set => _isMovingThrowPlatform = value;
    }

    private void Awake()
    {
        _platformEffector2D = GetComponent<PlatformEffector2D>();
        PlayerStatus.PlatformController = this;
    }

    private void OnEnable()
    {
        characterEvents.playerCrouchAndJumpOnPlatform.AddListener(playerCrouchAndJump);
    }

    private void OnDisable()
    {
        characterEvents.playerCrouchAndJumpOnPlatform.RemoveListener(playerCrouchAndJump);
    }
    

    private void playerCrouchAndJump(bool state)
    {
        _platformEffector2D.surfaceArc = 0;
        // noSurfaceTimer = noSurfaceTime;
        _noSurface = true;
        IsMovingThrowPlatform = true;
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.collider.CompareTag(TagStrings.playerTag))
        {
            // Logger.Log("player got hit");
            if (_noSurface)
            {
                IsMovingThrowPlatform = false;
                _platformEffector2D.surfaceArc = 180;
                _noSurface = false;
            }
        }
        
    }
}