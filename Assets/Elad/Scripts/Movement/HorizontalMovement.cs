using BitStrap;
using Elad.Events;
using Elad.Scripts;
using Elad.Scripts.Combat;
using Elad.Scripts.Events;
using Managers;
using Mechanics.Enemies;
using UnityEngine;
using UnityEngine.InputSystem;
using Logger = Nemesh.Logger;
using FMOD.Studio;

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
    private bool _crouchIsPush;

    public bool IsCrouching
    {
        get => _isCrouching;
        set
        {
            if (_isCrouching != value)
            {
                if (value)
                {
                    _playerFootsteps.setParameterByName(MusicStrings.FootStepsPitch, 1);

                }

                else
                {
                    _playerFootsteps.setParameterByName(MusicStrings.FootStepsPitch, 0);
                }
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
                ParticleEvents.PlayerChangeDirection.Invoke();
            }

            _isFacingRight = value;
        }
    }


    [SerializeField] [Range(5, 20)] private float knockBackMultiplayer = 1;

    [Header("Sounds")] [SerializeField] [Range(0, 0.1f)]
    private float stepsVolume = 0.05f;

    [SerializeField] [Range(0, 1)] private float stepsSoundGapTime = 0.1f;
    private float _stepsSoundGapTimer;
    private bool _stopStepSound;

    private EventInstance _playerFootsteps;

    private void OnEnable()
    {
        characterEvents.FunctionsLoad.AddListener(ResetMovement);
        characterEvents.PauseGame.AddListener(ResetMovement);
    }

    private void OnDisable()
    {
        characterEvents.FunctionsLoad.RemoveListener(ResetMovement);
        characterEvents.PauseGame.RemoveListener(ResetMovement);
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

    private void Start()
    {
        _playerFootsteps = AudioManager.instance.CreatEventInstance(FMODEvents.instance.playerFootsteps);
        SetGrassSurface();
    }

    public void OnCrouch(InputAction.CallbackContext context)
    {
        if (_playerController.CantGetInput()) return;

        if (_playerController.CanMove && !IsCrouching)
        {
            IsCrouching = true;
            _crouchIsPush = true;
        }
        else if (context.canceled)
        {
            if (!_touchingDirection.IsOnCeiling)
            {
                IsCrouching = false;
            }

            _crouchIsPush = false;
        }

        if (context.started && _onGround)
        {
            CameraManager.CrouchCameraController.SetOffset();
        }
        else if (context.canceled || !_onGround)
        {
            CameraManager.CrouchCameraController.ClearOffset();
        }
    }

    public void OnCrouch(bool state)
    {
        if (_playerController.CantGetInput()) return;
        IsCrouching = state;
    }

    public void OnMovement(InputAction.CallbackContext context)
    {
        if (_playerController.CantGetInput()) return;
        var direction = Vector2.zero;
        if (context.phase != InputActionPhase.Canceled && _playerController.CanMove)
        {
            direction = context.ReadValue<Vector2>();
        }


        DirectionX = direction.x;
        _playerController.IsMoving = (DirectionX != 0);
        SetFacingDirection(DirectionX);
    }

    private void ResetMovement()
    {
        DirectionX = 0;
        _playerController.IsMoving = (DirectionX != 0);
        IsCrouching = false;
    }

    public void CloseMovementToWall(float newMovement)
    {
    }

    public void OnRun(InputAction.CallbackContext context)
    {
        if (_playerController.CantGetInput()) return;
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
        if (GeneralGameManager.IsGamePause || !PlayerStatus.IsAlive) return;

        PlayerStatus.playerVelocity = _rB.velocity;
        var currentDirX = CanMove ? DirectionX : 0;


        _pressingMovementKey = (currentDirX != 0);

        //Must be after the line above because of the automate gliding horizontal movement
        currentDirX = PlayerStatus.IsGliding ? (IsFacingRight ? 1 : -1) : currentDirX;
        _desiredVelocity = new Vector2(currentDirX, 0f) * Mathf.Max(CurrentMoveSpeed - friction, 0f);

        CrouchHandler();
        UpdateFootStepsSound();
    }

    private void CrouchHandler()
    {
        if (IsCrouching)
        {
            // if (!_onGround)
            // {
            //     IsCrouching = false;
            // }
            if (!_crouchIsPush && !_touchingDirection.IsOnCeiling)
            {
                // Logger.Log("kaka");
                IsCrouching = false;
            }
        }
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

    public float DirectionX
    {
        get => directionX;
        set => directionX = value;
    }

    private void FixedUpdate()
    {
        _onGround = _touchingDirection.IsGrounded;
        if (!_onGround && CameraManager.CrouchCameraController != null)
        {
            CameraManager.CrouchCameraController.ClearOffset();
        }

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
            if (Mathf.Sign(DirectionX) != Mathf.Sign(_velocity.x))
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
        // print($"{_velocity.x} :: {_desiredVelocity.x} :: {_rB.velocity.x}");

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
        bool changedSides = false;
        if (movementInput > 0 && !IsFacingRight)
        {
            changedSides = true;
            IsFacingRight = true;
        }

        else if (movementInput < 0 && IsFacingRight)
        {
            changedSides = true;
            IsFacingRight = false;
        }

        if (changedSides && _touchingDirection.IsGrounded)
        {
            AudioManager.instance.PlayOneShot(FMODEvents.instance.playerTurnSide, transform.position);

        }

        PlayerStatus.isFacingRight = IsFacingRight;
    }

    public float GetHorizontalMovement()
    {
        return DirectionX;
    }


    public void OnHit(int damage, Vector2 knockBack, float delay = 0f)
    {
        if (_playerController.CantGetInput()) return;
        if (delay > 0)
        {
            StartCoroutine(CorotuineUtils.DelayExecution(delay,
                    () =>
                    {
                        knockBack *= knockBackMultiplayer;
                        _rB.AddForce(knockBack, ForceMode2D.Impulse);
                    }
                )
            );
            return;
        }

        knockBack *= knockBackMultiplayer;
        _rB.AddForce(knockBack, ForceMode2D.Impulse);
    }

    public void MoveRight()
    {
        if (GeneralGameManager.IsGamePause || !PlayerStatus.IsAlive || !_playerController.CanMove) return;

        DirectionX = 1; // 1 corresponds to moving to the right.
        _playerController.IsMoving = true;
        SetFacingDirection(DirectionX);
    }

    private void UpdateFootStepsSound()
    {
        _playerFootsteps.setParameterByName(MusicStrings.FootStepsVolume, stepsVolume);
        if (DirectionX != 0 && _touchingDirection.IsGrounded)
        {
            _stopStepSound = false;
            PLAYBACK_STATE playbackState;
            _playerFootsteps.getPlaybackState(out playbackState);
            if (playbackState.Equals(PLAYBACK_STATE.STOPPED))
            {
                _playerFootsteps.start();
            }
        }

        else
        {
            if (_stopStepSound)
            {
                _stepsSoundGapTimer -= Time.deltaTime;
                if (_stepsSoundGapTimer <= 0)
                {
                    _playerFootsteps.stop(STOP_MODE.ALLOWFADEOUT);
                    _stopStepSound = false;
                }
            }

            else
            {
                _stepsSoundGapTimer = stepsSoundGapTime;
                _stopStepSound = true;
            }
        }
    }

    [SerializeField] private MusicStrings.SurfaceSound surfaceSound = MusicStrings.SurfaceSound.Grass;

    private void SetGrassSurface()
    {
        _playerFootsteps.setParameterByName(MusicStrings.FootStepsSurfaceParam, (float) surfaceSound);
        _playerFootsteps.setParameterByName(MusicStrings.FootStepsVolume, stepsVolume);
    }

    [Button]
    public void ChangeSurface()
    {
        switch (surfaceSound)
        {
            case MusicStrings.SurfaceSound.Grass:
                surfaceSound = MusicStrings.SurfaceSound.WoodPlatform;
                break;
            case MusicStrings.SurfaceSound.WoodPlatform:
                surfaceSound = MusicStrings.SurfaceSound.Cave;
                break;
            case MusicStrings.SurfaceSound.Cave:
                surfaceSound = MusicStrings.SurfaceSound.Grass;
                break;
        }

        _playerFootsteps.setParameterByName(MusicStrings.FootStepsSurfaceParam, (float) surfaceSound);
        _playerFootsteps.setParameterByName(MusicStrings.FootStepsVolume, stepsVolume);
    }
}