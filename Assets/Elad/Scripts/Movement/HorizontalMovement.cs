using System;
using Elad.Events;
using Elad.Scripts;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using Logger = Nemesh.Logger;

public class HorizontalMovement : MonoBehaviour
{
    [Header("Components")] private PlayerController _playerController;
    private Rigidbody2D _rB;
    private TouchingDirection _touchingDirection;
    private Animator _animator;
    private SpecialMovements _specialMovements;
    private Damageable _damageable;
    private CharacterJump _playerJump;

    [Space(10)] [Header("Ground Movement")] [SerializeField, Range(0f, 20f)]
    private float maxSpeed = 10f;

    [SerializeField, Range(0f, 100f)] private float maxAcceleration = 52f;
    [SerializeField, Range(0f, 100f)] private float maxDeceleration = 52f; //Amount of time to stop

    [SerializeField, Range(0f, 100f)]
    private float maxTurnSpeed = 80f; //Amount of time to stop after changing direction

    [Space(10)] [Header("Air Movement")] [SerializeField, Range(0f, 100f)]
    private float maxAirAcceleration;

    [SerializeField, Range(0f, 100f)] private float maxAirDeceleration;

    [SerializeField, Range(0f, 100f)] private float maxAirTurnSpeed = 80f;

    [SerializeField] private float friction;

    [Space(10)] [Header("Options")] public bool useAcceleration;

    [Space(10)] [Header("Calculations")] [SerializeField]
    private float directionX;

    private Vector2 _desiredVelocity;
    private Vector2 _velocity;
    private float _maxSpeedChange;
    private float _acceleration;
    private float _deceleration;
    private float _turnSpeed;

    [Space(10)] [Header("Current State")] private bool _onGround;
    private bool _pressingMovementKey;

    [Space(10)] [Header("Running")] [SerializeField, Range(10f, 30f)]
    private float maxSpeedRunning = 20f;

    private bool _isRunning;

    private bool IsRunning
    {
        get => _isRunning;
        set
        {
            _isRunning = value;
            _animator.SetBool(AnimationStrings.isRunning, value);
        }
    }

    [Space(10)] [Header("Crouching")] private bool _isCrouching;
    [SerializeField, Range(0f, 10f)] private float maxSpeedCrouching = 5f;
    [SerializeField] private float crouchingWalkSpeed = 3f;

    public bool IsCrouching
    {
        get => _isCrouching;
        set
        {
            if (_isCrouching != value)
            {
                _animator.SetBool(AnimationStrings.isCrouching, value);

                _isCrouching = value;
                _playerController.ChangeCollider(value
                    ? PlayerController.ColliderKind.Circle
                    : PlayerController.ColliderKind.Capsule);
            }
        }
    }

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


    private void Awake()
    {
        PlayerStatus.isFacingRight = (transform.localScale.x > 0);
        _rB = GetComponent<Rigidbody2D>();
        _playerController = GetComponent<PlayerController>();
        _touchingDirection = GetComponent<TouchingDirection>();
        _animator = GetComponent<Animator>();
        _specialMovements = GetComponent<SpecialMovements>();
        _damageable = GetComponent<Damageable>();
        _playerJump = GetComponent<CharacterJump>();
    }

    public void OnCrouch(InputAction.CallbackContext context)
    {
        if (!context.canceled)
        {
            if (_playerController.CanMove && !IsCrouching)
                IsCrouching = true;
        }


        else if (context.canceled)
            IsCrouching = false;
    }

    public void OnCrouch(bool state)
    {
        IsCrouching = state;
    }

    public void OnMovement(InputAction.CallbackContext context)
    {
        if (PlayerStatus.isGamePause) return;
        var direction = Vector2.zero;
        if (context.phase != InputActionPhase.Canceled && _playerController.CanMove)
        {
            direction = context.ReadValue<Vector2>();
        }


        directionX = direction.x;
        _playerController.IsMoving = (directionX != 0);
        SetFacingDirection(directionX);
    }

    public void CloseMovementToWall(float newMovement)
    {
    }

    public void OnRun(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            IsRunning = true;
        }
        else if (context.canceled)
        {
            IsRunning = false;
        }
    }

    private bool CanMove => (_playerController.CanMove && _playerController.IsAlive && !_damageable.LockVelocity);

    private void Update()
    {
        PlayerStatus.playerVelocity = _rB.velocity;
        var currentDirX = CanMove ? directionX : 0;


        _pressingMovementKey = (currentDirX != 0);

        //Must be after the line above because of the automate gliding horizontal movement
        currentDirX = PlayerStatus.IsGliding ? (IsFacingRight ? 1 : -1) : currentDirX;
        _desiredVelocity = new Vector2(currentDirX, 0f) * Mathf.Max(CurrentMoveSpeed - friction, 0f);
    }

    private float CurrentMoveSpeed
    {
        get
        {
            if (_specialMovements.CurrentMovementStatus != SpecialMovements.MovementStatus.None)
            {
                return _specialMovements.CurrentSpeed;
            }

            if (PlayerStatus.IsGliding)
            {
                if (_pressingMovementKey)
                {
                    return _playerJump.GlideHorizontallyMovement; 
                }
                else
                {
                    return _playerJump.GlideHorizontallyMovementStatic;    
                }
                
            }

            if (IsCrouching)
                return maxSpeedCrouching;

            if (IsRunning)
                return maxSpeedRunning;


            return maxSpeed;
        }
    }

    private void FixedUpdate()
    {
        _onGround = _touchingDirection.IsGrounded;

        _velocity = _rB.velocity;

        if (useAcceleration)
        {
            RunWithAcceleration();
        }
        else
        {
            if (_onGround)
            {
                RunWithoutAcceleration();
            }
            else
            {
                RunWithAcceleration();
            }
        }
    }

    private void RunWithAcceleration()
    {
        //Set our _acceleration, _deceleration, and turn speed stats, based on whether we're on the ground on in the air
        _acceleration = _onGround ? maxAcceleration : maxAirAcceleration;
        _deceleration = _onGround ? maxDeceleration : maxAirDeceleration;
        _turnSpeed = _onGround ? maxTurnSpeed : maxAirTurnSpeed;

        if (_pressingMovementKey || PlayerStatus.IsGliding)
        {
            //If the sign (i.e. positive or negative) of our input direction doesn't match our movement,
            //it means we're turning around and so should use the turn speed stat.
            if (Mathf.Sign(directionX) != Mathf.Sign(_velocity.x))
            {
                _maxSpeedChange = _turnSpeed * Time.deltaTime;
            }
            else
            {
                //If they match, it means we're simply running along and so should use the _acceleration stat
                _maxSpeedChange = _acceleration * Time.deltaTime;
            }
        }
        else
        {
            //And if we're not pressing a direction at all, use the _deceleration stat

            _maxSpeedChange = _deceleration * Time.deltaTime;
        }

        //Move our _velocity towards the desired _velocity, at the rate of the number calculated above
        _velocity.x = Mathf.MoveTowards(_velocity.x, _desiredVelocity.x, _maxSpeedChange);

        //Update the Rigidbody with this new _velocity
        _rB.velocity = _velocity;
    }

    private void RunWithoutAcceleration()
    {
        _velocity.x = _desiredVelocity.x;
        _rB.velocity = _velocity;
    }

    public void SetFacingDirection(float movementInput)
    {
        if (movementInput > 0 && !IsFacingRight)
        {
            IsFacingRight = true;
        }

        else if (movementInput < 0 && IsFacingRight)
        {
            IsFacingRight = false;
        }

        PlayerStatus.isFacingRight = IsFacingRight;
    }

    public float GetHorizontalMovement()
    {
        return directionX;
    }


    public void OnHit(int damage, Vector2 knockBack)
    {
        float xKnockBack = _isFacingRight ? -knockBack.x : knockBack.x;
        xKnockBack = MathF.Abs(_rB.velocity.x) > 0.1 ? _rB.velocity.x + xKnockBack : 0;
        float yKnockBack = _rB.velocity.y + knockBack.y;
        _rB.velocity = new Vector2(xKnockBack, yKnockBack);
    }
}