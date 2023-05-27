using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallMovement : MonoBehaviour
{
    [Header("Components")] private TouchingDirection _touchingDirection;
    
    private Rigidbody2D _rB;
    private HorizontalMovement _horizontalMovement;
    private Animator _animator;

    private bool _wantToWallSlide;
    private bool _isWallSliding;

    
    public bool IsWallSliding
    {
        get
        {
            return _isWallSliding;
        }

        set
        {
            
            _isWallSliding = value;
            _animator.SetBool(AnimationStrings.isWallSliding, value);
            _rB.drag = value ? linearDragWallSliding : _linearDragRegular;
        }
    }

    
    

    [SerializeField, Range(0f, 1f)] [Tooltip("linear Drag to apply when sliding")]
    private float linearDragWallSliding = 2f;
    private float _linearDragRegular;
    
    [SerializeField, Range(0f, 5f)] [Tooltip("Gravity multiplier to apply when sliding")]
    private float gravityMultiplierWallSliding = 0.3f;
    
    public float GravityMultiplierWallSliding
    {
        get => gravityMultiplierWallSliding;
        set => gravityMultiplierWallSliding = value;
    }

    [SerializeField] private float xPowerWhenJump = 2f;
    public float XPowerWhenJump
    {
        get => xPowerWhenJump;
        set => xPowerWhenJump = value;
    }

    [SerializeField, Range(0f, 0.3f)] [Tooltip("How long should coyote time last?")]
    private float coyoteTime = 0.15f;

    private float _coyoteTimer;

    
    private void Awake()
    {
        _horizontalMovement = GetComponent<HorizontalMovement>();
        _touchingDirection = GetComponent<TouchingDirection>();
        _rB = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _linearDragRegular = _rB.drag;

    }


    private void Update()
    {
        WallCheck();
        IsWallSliding = _wantToWallSlide;    
        
        
    }

    void WallCheck()
    {
        if (_touchingDirection.IsOnWall &&
            !(_touchingDirection.IsGrounded) &&
            (_rB.velocity.y < 0.1f) &&
            (Mathf.Abs(_horizontalMovement.GetHorizontalMovement()) != 0)
        )
        {
            
            _wantToWallSlide = true;
            _coyoteTimer = coyoteTime;
        }

        else
        {

            _coyoteTimer -= Time.deltaTime;
            if (_coyoteTimer <= 0)
            {
                _wantToWallSlide = false;    
            }
            
        }
    }
    
    
    
}