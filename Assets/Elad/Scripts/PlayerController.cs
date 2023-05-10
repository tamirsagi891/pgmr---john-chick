using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

[RequireComponent(typeof(Rigidbody2D), typeof(TouchingDirection))]
public class PlayerController : MonoBehaviour
{
    

    [Space(10)] [Header("Gliding")] private bool _canGlide = true;

    private bool _isGliding;
    [Range(0, 1)] [SerializeField] private float gravityPercentagesDuringGlide = 0.5f;

    private float _originalGravity;

    [Space(10)] [Header("Movement")] [SerializeField]
    private float airWalkSpeed = 3f;

    [SerializeField] private float walkSpeed = 5f;
    private Vector2 _movementInput;
    private bool _isMoving;

    public bool IsMoving
    {
        get => _isMoving;
        set
        {
            _isMoving = value;
            _animator.SetBool(AnimationStrings.isMoving, value);
        }
    }

    [Space(10)] [Header("Touching")] private TouchingDirection _touchingDirection;

    [Space(10)] [Header("Jumping")] [SerializeField]
    private float jumpImpulse = 5f;

    [SerializeField] private float doubleJumpImpulse = 5f;
    private bool _canDoubleJump = true;

    [Space(10)] [Header("Components")] private Rigidbody2D _rB;
    private Animator _animator;

    [Space(10)] [Header("Collider")] private CapsuleCollider2D _capsuleCollider2D;
    private CircleCollider2D _circleCollider2D;

    public enum ColliderKind
    {
        Capsule,
        Circle,
        DodgeRoll
    }

    [Space(10)] [Header("Wall Movement")] [SerializeField]
    private bool _isWallSliding;

    [SerializeField] private float wallSlidingSpeed = 2f;

    private bool _isInWallJump;
    private bool _wallJump;
    [SerializeField] private Vector2 wallJumpingPower = new Vector2(8f, 4f);
    [SerializeField] private float wallJumpingTime = 0.3f;
    private float _wallJumpingTimer;

    private float CurrentMoveSpeed
    {
        get
        {
            if (CanMove)
            {
                
                if (_isMoving && !_touchingDirection.IsOnWall)
                {
                    if (_touchingDirection.IsGrounded)
                    {
                       
                    }


                    else
                    {
                        return 0;
                    }
                }
                else
                {
                    //Movement lock
                    return 0;
                }
            }

            return 0;
        }
    }


    private void Awake()
    {
        _rB = GetComponent<Rigidbody2D>();
        _originalGravity = _rB.gravityScale;
        _animator = GetComponent<Animator>();
        _touchingDirection = GetComponent<TouchingDirection>();
        _capsuleCollider2D = GetComponent<CapsuleCollider2D>();
        _circleCollider2D = GetComponent<CircleCollider2D>();
        ChangeCollider(ColliderKind.Capsule);
    }

    private void FixedUpdate()
    {
        // if (_isInWallJump)
        // {
        //     WallJump();
        //     return;
        // }
        //
        // // var addVel = new Vector2(_rB.mass* _movementInput.x * CurrentMoveSpeed, 0); 
        // // _rB.AddForce(addVel, ForceMode2D.Impulse);
        // _rB.velocity = new Vector2(_movementInput.x * CurrentMoveSpeed, _rB.velocity.y);
        // if (_movementInput.x == 0 && _isDashing)
        // {
        //     _rB.velocity = new Vector2(transform.localScale.x * CurrentMoveSpeed, _rB.velocity.y);
        // }
        //
        // _animator.SetFloat(AnimationStrings.yVelocity, _rB.velocity.y);
        // if (_touchingDirection.IsGrounded)
        //     _canDoubleJump = true;
        //
        // WallSlide();
    }

    private void WallJump()
    {
        if (!_wallJump)
        {
            _wallJumpingTimer = wallJumpingTime;
            _wallJump = true;
            int direction = -Mathf.FloorToInt(Mathf.Sign(_movementInput.x));
            var addVel = new Vector2(direction * wallJumpingPower.x, wallJumpingPower.y);
            _rB.AddForce(addVel, ForceMode2D.Impulse);
        }

        _wallJumpingTimer -= Time.deltaTime;
        if (_wallJumpingTimer < 0)
        {
            _isInWallJump = false;
            _wallJump = false;
        }
    }

    private void WallSlide()
    {
        //With pressing move to the wall
        if (_movementInput.x != 0 && _touchingDirection.IsOnWall && !_touchingDirection.IsGrounded)
            // if (  _touchingDirection.IsOnWall && !_touchingDirection.IsGrounded) //Without 
        {
            if (!_isWallSliding)
            {
                _animator.SetBool(AnimationStrings.isWallSliding, true);
                _isWallSliding = true;
                _rB.velocity = new Vector2(0, -wallSlidingSpeed);
                _rB.gravityScale = 0;
            }
        }

        else
        {
            StopWallSlide();
        }

        if (_touchingDirection.IsGrounded)
        {
            StopWallSlide();
        }
    }

    private void StopWallSlide()
    {
        if (_isWallSliding)
        {
            _animator.SetBool(AnimationStrings.isWallSliding, false);
            _isWallSliding = false;
            _rB.gravityScale = _originalGravity;
        }
    }


    private bool CanGlide
    {
        get
        {
            var returnValue = !(_touchingDirection.IsGrounded);
            return returnValue;
        }
    }
    
    public bool CanMove
    {
        get { return _animator.GetBool(AnimationStrings.canMove); }
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        //TODO:: CHECK IF ALIVE
        if (context.started && CanMove)
        {
            if (_isWallSliding && !_isInWallJump)
                _isInWallJump = true;

            if (_touchingDirection.IsGrounded)
            {
                _animator.SetTrigger(AnimationStrings.jumpTrigger);
                _rB.velocity = new Vector2(_rB.velocity.x, jumpImpulse);
            }

            else
            {
                if (_canDoubleJump)
                {
                    _animator.SetTrigger(AnimationStrings.doubleJumpTrigger);
                    var doubleJumpForce = new Vector2(0, doubleJumpImpulse);
                    _rB.AddForce(doubleJumpForce, ForceMode2D.Impulse);
                    _canDoubleJump = false;
                }
            }
        }
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            _animator.SetTrigger(AnimationStrings.attackTrigger);
        }
    }
    
    public void OnGlide(InputAction.CallbackContext context)
    {
        if (context.started && CanGlide && !_isGliding)
        {
            _rB.velocity = new Vector2(_movementInput.x * CurrentMoveSpeed, 0);

            _rB.gravityScale = _originalGravity * gravityPercentagesDuringGlide;
            _animator.SetBool(AnimationStrings.isGliding, true);
            _isGliding = true;
        }

        if (context.canceled && _isGliding)
        {
            _animator.SetBool(AnimationStrings.isGliding, false);
            _rB.gravityScale = _originalGravity;
            _isGliding = false;
        }
    }

    public void ChangeCollider(ColliderKind colliderKind)
    {
        switch (colliderKind)
        {
            case ColliderKind.Capsule:
                _capsuleCollider2D.enabled = true;
                _circleCollider2D.enabled = false;
                break;

            case ColliderKind.Circle:
                _capsuleCollider2D.enabled = false;
                _circleCollider2D.enabled = true;
                break;

            case ColliderKind.DodgeRoll:
                break;
        }
    }
}