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
    [Space(10)] [Header("Dashing")] private bool _canDash = true;
    private bool _isDashing;
    [SerializeField] private float dashingSpeed = 24f;
    private float dashingTime = 0.2f;
    private float dashingCoolDown = 1f;
    private TrailRenderer tr;

    [Space(10)] [Header("Dodge Roll")] private bool _canDodgeRoll = true;
    private bool _isDodgeRoll;
    [SerializeField] private float _dodgeRollSpeed = 24f;
    private float dodgeRollTime = 0.2f;
    private float dodgeRollCoolDown = 1f;

    [Space(10)] [Header("Gliding")] private bool _canGlide = true;

    private bool _isGliding;
    [Range(0, 1)] [SerializeField] private float gravityPercentagesDuringGlide = 0.5f;

    private float _originalGravity;

    [Space(10)] [Header("Movement")] [SerializeField]
    private float airWalkSpeed = 3f;

    [SerializeField] private float walkSpeed = 5f;
    private Vector2 _movementInput;
    private bool _isMoving;

    private bool IsMoving
    {
        get => _isMoving;
        set
        {
            _isMoving = value;
            _animator.SetBool(AnimationStrings.isMoving, value);
        }
    }

    private bool _isRunning;
    [SerializeField] private float runSpeed = 8f;

    public bool IsRunning
    {
        get => _isRunning;
        private set
        {
            _isRunning = value;
            _animator.SetBool(AnimationStrings.isRunning, value);
        }
    }

    [Space(10)] [Header("Touching")] private TouchingDirection _touchingDirection;

    [Space(10)] [Header("Facing")] private bool _isFacingRight = true;

    private bool IsFacingRight
    {
        get => _isFacingRight;
        set
        {
            if (_isFacingRight != value)
            {
                transform.localScale *= new Vector2(-1, 1);
            }

            _isFacingRight = value;
        }
    }

    [Space(10)] [Header("Jumping")] [SerializeField]
    private float jumpImpulse = 5f;

    [SerializeField] private float doubleJumpImpulse = 5f;
    private bool _canDoubleJump = true;


    [Space(10)] [Header("Crouching")] private bool _isCrouching;
    [SerializeField] private float crouchingWalkSpeed = 3f;

    private bool IsCrouching
    {
        get => _isCrouching;
        set
        {
            if (_isCrouching != value)
            {
                
                _animator.SetBool(AnimationStrings.isCrouching, value);
            }

            _isCrouching = value;

            ChangeCollider(value ? ColliderKind.Circle : ColliderKind.Capsule);
        }
    }

    [Space(10)] [Header("Components")] private Rigidbody2D _rB;
    private Animator _animator;

    [Space(10)] [Header("Collider")] private CapsuleCollider2D _capsuleCollider2D;
    private CircleCollider2D _circleCollider2D;

    enum ColliderKind
    {
        Capsule,
        Circle,
        DodgeRoll
    }

    // [Space(10)] [Header("Wall Movement")]
    
    private float CurrentMoveSpeed
    {
        get
        {
            if (CanMove)
            {
                if (_isDodgeRoll)
                {
                    return _dodgeRollSpeed;
                }

                if (_isDashing)
                {
                    return dashingSpeed;
                }

                if (_isMoving && !_touchingDirection.IsOnWall)
                {
                    if (_touchingDirection.IsGrounded)
                    {
                        if (IsCrouching)
                        {
                            return crouchingWalkSpeed;
                        }

                        if (IsRunning)
                        {
                            return runSpeed;
                        }
                        else
                        {
                            return walkSpeed;
                        }
                    }
                    else
                    {
                        return airWalkSpeed;
                    }
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
    }


    private void Awake()
    {
        _rB = GetComponent<Rigidbody2D>();
        _originalGravity = _rB.gravityScale;
        _animator = GetComponent<Animator>();
        _touchingDirection = GetComponent<TouchingDirection>();
        tr = GetComponent<TrailRenderer>();
        _capsuleCollider2D = GetComponent<CapsuleCollider2D>();
        _circleCollider2D = GetComponent<CircleCollider2D>();
        ChangeCollider(ColliderKind.Capsule);
    }

    private void FixedUpdate()
    {
        
        _rB.velocity = new Vector2(_movementInput.x * CurrentMoveSpeed, _rB.velocity.y);
        if (_movementInput.x == 0 && _isDashing)
        {
            _rB.velocity = new Vector2(transform.localScale.x * CurrentMoveSpeed, _rB.velocity.y);
        }

        _animator.SetFloat(AnimationStrings.yVelocity, _rB.velocity.y);
        if (_touchingDirection.IsGrounded)
            _canDoubleJump = true;
        WallHandler();
    }

    private void WallHandler()
    {
        if (_touchingDirection.IsOnWall)
        {
            
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


    public void OnMove(InputAction.CallbackContext context)
    {
        if (_isDashing) return;
        _movementInput = context.ReadValue<Vector2>();
        IsMoving = (_movementInput != Vector2.zero);

        SetFacingDirection(_movementInput);
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (_isDashing) return;
        //TODO:: CHECK IF ALIVE

        if (context.started && CanMove)
        {
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

    public void OnCrouch(InputAction.CallbackContext context)
    {
        if (_isDashing) return;

        
        if (context.started && CanMove && !IsCrouching && _touchingDirection.IsGrounded)
        {
            IsCrouching = true;
        }

        else if (context.canceled)
        {
            IsCrouching = false;
        }
    }

    private void SetFacingDirection(Vector2 movementInput)
    {
        if (movementInput.x > 0 && !IsFacingRight)
        {
            IsFacingRight = true;
        }

        else if (movementInput.x < 0 && IsFacingRight)
        {
            IsFacingRight = false;
        }
    }


    public void OnRun(InputAction.CallbackContext context)
    {
        if (_isDashing) return;
        if (context.started)
        {
            IsRunning = true;
        }
        else if (context.canceled)
        {
            IsRunning = false;
        }
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            _animator.SetTrigger(AnimationStrings.attackTrigger);
        }
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        if (context.started && _canDash)
        {
            StartCoroutine(Dash());
        }
    }

    public void OnDodgeRoll(InputAction.CallbackContext context)
    {
        if (context.started && _canDodgeRoll)
        {
            StartCoroutine(DodgeRoll());
        }
    }

    public void OnGlide(InputAction.CallbackContext context)
    {
        if (context.started && CanGlide)
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

    private IEnumerator Dash()
    {
        _canDash = false;
        _isDashing = true;
        _rB.gravityScale = 0;
        tr.emitting = true;
        _animator.SetTrigger(AnimationStrings.dashTrigger);


        yield return new WaitForSeconds(dashingTime);

        tr.emitting = false;
        _rB.gravityScale = _originalGravity;
        _isDashing = false;

        yield return new WaitForSeconds(dashingCoolDown);
        _canDash = true;
    }

    private IEnumerator DodgeRoll()
    {
        _canDodgeRoll = false;
        _isDodgeRoll = true;
        _animator.SetTrigger(AnimationStrings.dodgeRollTrigger);
        yield return new WaitForSeconds(dodgeRollTime);
        ChangeCollider(ColliderKind.DodgeRoll);

        _isDodgeRoll = false;

        yield return new WaitForSeconds(dodgeRollCoolDown);
        ChangeCollider(ColliderKind.Capsule);
        _canDodgeRoll = true;
    }

    private void ChangeCollider(ColliderKind colliderKind)
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