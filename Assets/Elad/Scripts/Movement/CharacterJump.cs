using System;
using System.Collections.Generic;
using Elad.Events;
using Elad.Scripts;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using Logger = Nemesh.Logger;
using System;
using BitStrap;
using UnityEngine;

//This script handles moving the character on the Y axis, for jumping and gravity

public class CharacterJump : MonoBehaviour
{
    [Header("Components")] private Rigidbody2D _rB;

    private WallMovement _wallMovement;
    private TouchingDirection _touchingDirection;
    private Vector2 _velocity;
    private HorizontalMovement _horizontalMovement;
    private PlayerController _playerController;
    private Animator _animator;


    [Header("Jumping Stats")] [SerializeField, Range(2f, 5.5f)]
    private float maxJumpHeight = 7.3f;

    [SerializeField, Range(0.2f, 1.25f)] private float timeToReachPeakHeight;

    [SerializeField, Range(0f, 5f)] [Tooltip("Gravity multiplier to apply when going up")]
    private float gravityMultiplierAscending = 1f;

    [SerializeField, Range(1f, 50f)] [Tooltip("Gravity multiplier to apply when coming down")]
    private float gravityMultiplierDescending = 6.17f;

    [Header("Options")] [SerializeField] private bool canDoubleJump;
    [SerializeField] private bool dropWhenStopPushingJump;

    [SerializeField, Range(1f, 10f)] private float gravityPercentLetGoJump;

    [SerializeField] [Tooltip("The fastest speed the character can fall")]
    private float maxFallSpeed;

    [SerializeField, Range(0f, 1f)] [Tooltip("How long should coyote time last?")]
    private float coyoteTime = 0.15f;

    [SerializeField, Range(0f, 0.3f)] [Tooltip("How far from ground should we cache your jump?")]
    private float jumpBuffer = 0.15f;

    [Header("Double Jump")] [SerializeField, Range(0f, 2f)]
    private float doubleJumpMultiplier = 1.5f;

    private bool _canDoubleJump;

    [Header("Calculations")] private float _jumpSpeed;
    private float _defaultGravityScale;
    private float _gravMultiplier;

    [Header("Current State")] public bool canJumpAgain = false;
    private bool _desiredJump;
    private float _jumpBufferCounter;
    [SerializeField]
    [ReadOnly]
    private float _coyoteTimeCounter = 0;
    private bool _pressingJump;
    private bool _onGround;
    private bool _currentlyJumping;

    [Header("Gliding")] [SerializeField][Tooltip("Horizontal speed without holding the key")] private float glideHorizontallyMovementStaticStatic = 5f;
    [SerializeField][Tooltip("Horizontal speed with holding the key")] private float glideHorizontallyMovement = 5f;
    [SerializeField] private bool regularGlide = true;
    [SerializeField] private Vector2 glideJump = Vector2.zero;

    [SerializeField, Range(0f, 10f)] [Tooltip("Gravity multiplier to apply when gliding")]
    private float gravityMultiplierGliding = 0.3f;

    private bool _wantToGlide;

    [SerializeField] [Range(-0.01f, -0.2f)]
    private float minVelocityToGlide = -0.05f;

    [SerializeField, Range(0f, 20f)] [Tooltip("linear Drag to apply when gliding")]
    private float linearDragGliding = 2f;

    private float linearDragRegular;

    private int counterTest = 0;
    private bool _canGlide = true;
    private bool _isGliding;

    private bool CanGlide
    {
        get
        {
            var returnValue = (!(_touchingDirection.IsGrounded) && (!PlayerStatus.IsMovingThrowPlatform));
            returnValue = returnValue && !((_wallMovement.IsWallSliding || _inHit));
            
            return returnValue;
        }
    }

    public bool IsGliding
    {
        get => _isGliding;
        set => _isGliding = value;
    }

    [Space(10)] [Header("Crouch Affect jump")] [Tooltip("Let the player to jump from crouching")] [SerializeField]
    private bool canJumpWhileCrouch;

    private bool _canJump = true;

    public bool CanJump
    {
        get
        {
            var _isCrouching = false;
            if (!canJumpWhileCrouch)
            {
                _isCrouching = _horizontalMovement.IsCrouching;
            }

            var returnValue = (OnGround || (_coyoteTimeCounter < coyoteTime) || canJumpAgain ||
                               _wallMovement.IsWallSliding) && (!_isCrouching) && !_inHit;
            print($"{OnGround} || {_coyoteTimeCounter < coyoteTime} || {canJumpAgain} || {_wallMovement.IsWallSliding}");
            return returnValue;
        }
    }

    public float GlideHorizontallyMovementStatic
    {
        get => glideHorizontallyMovementStaticStatic;
        set => glideHorizontallyMovementStaticStatic = value;
    }

    public float GlideHorizontallyMovement
    {
        get => glideHorizontallyMovement;
        set => glideHorizontallyMovement = value;
    }

    public bool OnGround
    {
        get => _touchingDirection.IsGrounded;
        set => _touchingDirection.IsGrounded = value;
    }

    [Space(3)] [Header("On Hit")] [SerializeField]
    private float hitGlideDelayTime = 0.05f;

    private float _hitGlideDelayTimer;
    private bool _inHit;

    private void OnEnable()
    {
        characterEvents.CharacterDamaged.AddListener(StopGlideFromHit);
    }

    private void OnDisable()
    {
        characterEvents.CharacterDamaged.RemoveListener(StopGlideFromHit);
    }

    void Awake()
    {
        //Find the character's Rigidbody and ground detection
        _rB = GetComponent<Rigidbody2D>();
        _touchingDirection = GetComponent<TouchingDirection>();
        _defaultGravityScale = 1f;
        _playerController = GetComponent<PlayerController>();
        _animator = GetComponent<Animator>();
        linearDragRegular = _rB.drag;
        _wallMovement = GetComponent<WallMovement>();
        _horizontalMovement = GetComponent<HorizontalMovement>();
        PlayerStatus.JumpController = this;
    }

    private void OnDestroy()
    {
        if (PlayerStatus.JumpController == this)
        {
            PlayerStatus.JumpController = null;
        }
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.canceled)
        {
            _pressingJump = false;
        }
        //This function is called when one of the jump buttons (like space or the A button) is pressed.
        if (_playerController.CanMove)
        {
            //When we press the jump button, tell the script that we desire a jump.
            //Also, use the started and canceled contexts to know if we're currently holding the button
            if (context.started)
            {
                _pressingJump = true;
                if (_horizontalMovement.IsCrouching)
                {
                    if (_touchingDirection.IsOnPlatform)
                    {
                        _animator.SetTrigger(AnimationStrings.crouchToFallTrigger);
                        characterEvents.playerCrouchAndJumpOnPlatform.Invoke(true);
                        return;
                    }
                }

                if (CanJump)
                {
                    _desiredJump = true;
                }
                
            }
        }
    }


    void Update()
    {
        OnGlide();
        IsGliding = !_touchingDirection.IsGrounded && IsGliding;
        if (_inHit)
        {
            _hitGlideDelayTimer -= Time.deltaTime;
            if (_hitGlideDelayTimer <= 0)
            {
                _inHit = false;
            }
        }
        
        //Jump buffer allows us to queue up a jump, which will play when we next hitTrigger the ground
        if (jumpBuffer > 0)
        {
            //Instead of immediately turning off "desireJump", start counting up...
            //All the while, the DoAJump function will repeatedly be fired off
            if (_desiredJump)
            {
                _jumpBufferCounter += Time.deltaTime;

                if (_jumpBufferCounter > jumpBuffer)
                {
                    //If time exceeds the jump buffer, turn off "desireJump"
                    _desiredJump = false;
                    _jumpBufferCounter = 0;
                }
            }
        }

        //If we're not on the ground and we're not currently jumping, that means we've stepped off the edge of a platform.
        //So, start the coyote time counter...
        
        if (!_currentlyJumping && !OnGround && !_wallMovement.IsWallSliding)
        {
            _coyoteTimeCounter += Time.deltaTime;
        }

        if (!_currentlyJumping && (OnGround || _wallMovement.IsWallSliding))
        {
            _coyoteTimeCounter = 0;
        }
    }

    private void setPhysics()
    {
        _rB.gravityScale = GetGravityScale(_gravMultiplier);
    }

    private float GetGravityScale(float mult)
    {

        //Determine the character's gravity scale, using the stats provided. Multiply it by a _gravMultiplier, used later
        Vector2 newGravity = new Vector2(0, (-2 * maxJumpHeight) / (timeToReachPeakHeight * timeToReachPeakHeight));
        var a = (newGravity.y / Physics2D.gravity.y) * mult;
        return a;
    }

    private void FixedUpdate()
    {
        //Get _velocity from Kit's Rigidbody 
        _velocity = _rB.velocity;
        calculateGravity();
        setPhysics();

        //Keep trying to do a jump, for as long as _desiredJump is true
        if (_desiredJump)
        {
            DoAJump();
            _rB.velocity = _velocity;

            //Skip gravity calculations this frame, so _currentlyJumping doesn't turn off
            //This makes sure you can't do the coyote time double jump bug
            return;
        }

        _animator.SetFloat(AnimationStrings.yVelocity, _rB.velocity.y);
    }

    private void calculateGravity()
    {
        //We change the character's gravity based on her Y direction

        if (IsGliding)
        {
            _gravMultiplier = gravityMultiplierGliding;
        }
        else if (_wallMovement.IsWallSliding)        //If Kit is going up...
        {
            _gravMultiplier = _wallMovement.GravityMultiplierWallSliding;
        }
        else if (_rB.velocity.y > 0.01f)
        {
            calculateGravityUp();
        }
        else if (_rB.velocity.y < -0.01f && !OnGround)        //Else if going down...
        {
            calculateGravityDown();
        }
        else  //Else not moving vertically at all
        {
            _currentlyJumping = false;
            _gravMultiplier = _defaultGravityScale;
        }

        //Set the character's Rigidbody's _velocity
        //But clamp the Y variable within the bounds of the speed limit, for the terminal _velocity assist option

        _rB.velocity = new Vector3(_velocity.x, Mathf.Clamp(_velocity.y, -maxFallSpeed, 100));
    }

    private void calculateGravityUp()
    {
        //If we're using variable jump height...)
        if (dropWhenStopPushingJump)
        {
            //Apply upward multiplier if player is rising and holding jump
            if (_pressingJump && _currentlyJumping)
            {
                _gravMultiplier = gravityMultiplierAscending;
            }
            //But apply a special downward multiplier if the player lets go of jump
            else
            {
                _gravMultiplier = gravityPercentLetGoJump;
            }
        }
        else
        {
            _gravMultiplier = gravityMultiplierAscending;
        }
    }

    private void calculateGravityDown()
    {
        _gravMultiplier = gravityMultiplierDescending;
    }


    private void DoAJump()
    {
        // Create the jump, provided we are on the ground, in coyote time, or have a double jump available
        if (CanJump)
        {
            _desiredJump = false;
            _jumpBufferCounter = 0;
            _coyoteTimeCounter = coyoteTime + 1f;

            // Determine the power of the jump, based on our gravity and stats
            var gScale = GetGravityScale(_wallMovement.IsWallSliding ? _wallMovement.GravityMultiplierWallSliding : _defaultGravityScale);
            _jumpSpeed = Mathf.Sqrt(-2f * Physics2D.gravity.y * gScale * maxJumpHeight);
            
            // If we have double jump on, allow us to jump again (but only once)
            if (canDoubleJump)
            {
                bool wantToDoubleJump = (canJumpAgain && (!_touchingDirection.IsGrounded));
                canJumpAgain = !wantToDoubleJump;
                var whichJumpAnimation = AnimationStrings.jumpTrigger;
                // Apply double jump multiplier if it's a double jump
                if (wantToDoubleJump)
                {
                    _jumpSpeed *= doubleJumpMultiplier;
                    whichJumpAnimation = AnimationStrings.doubleJumpTrigger;
                }

                _animator.SetTrigger(whichJumpAnimation);
            }
            else
            {
                _animator.SetTrigger(AnimationStrings.jumpTrigger);
            }


            // If Kit is moving up or down when she jumps (such as when doing a double jump), change the _jumpSpeed;
            // This will ensure the jump is the exact same strength, no matter your _velocity.
            if (_velocity.y > 0f)
            {
                _jumpSpeed = Mathf.Max(_jumpSpeed - _velocity.y, 0f);
            }
            else if (_velocity.y < 0f)
            {
                _jumpSpeed += Mathf.Abs(_velocity.y);
            }

            float xAddVelocity = 0;
            if (_wallMovement.IsWallSliding && _touchingDirection.IsOnWall)
            {
                xAddVelocity = PlayerStatus.isFacingRight
                    ? -_wallMovement.XPowerWhenJump
                    : _wallMovement.XPowerWhenJump;
                _horizontalMovement.CloseMovementToWall(xAddVelocity);
                _wallMovement.IsWallSliding = false;
                // _horizontalMovement.SetFacingDirection(-xAddVelocity);
            }
            
            _velocity.y += _jumpSpeed;
            _velocity.x += xAddVelocity;
            _currentlyJumping = true;
        }

        if (jumpBuffer == 0)
        {
            // If we don't have a jump buffer, then turn off _desiredJump immediately after hitting jumping
            _desiredJump = false;
        }
    }


    public void bounceUp(float bounceAmount)
    {
        //Used by the springy pad
        _rB.AddForce(Vector2.up * bounceAmount, ForceMode2D.Impulse);
    }


    private void OnGlide()
    {
        if (!CanGlide)
        {
            CancelGlide();
            return;
        }

        if (regularGlide)
        {
            if (_rB.velocity.y < minVelocityToGlide)
            {
                if (!IsGliding && _pressingJump)
                {
                    _rB.drag = linearDragGliding;
                    _animator.SetBool(AnimationStrings.isGliding, true);
                    IsGliding = true;
                }
            }

            CancelGlide();
        }

        else
        {
            SpecialGlide(); 
        }
    }

    private void CancelGlide()
    {
        if (IsGliding && (!_pressingJump || !CanGlide))
        {
            _rB.drag = linearDragRegular;
            _animator.SetBool(AnimationStrings.isGliding, false);
            IsGliding = false;
        }
    }

    private void SpecialGlide()
    {
        if (_rB.velocity.y < 0f)
        {
            if (CanGlide && !IsGliding && _pressingJump)
            {
                _horizontalMovement.OnHit(0, glideJump);
                _rB.drag = linearDragGliding;
                Logger.Log("kakakakak");
                // _animator.SetBool(AnimationStrings.isGliding, true);
                IsGliding = true;
                Logger.Log("CCCC");
            }

            Logger.Log("BBBB");
        }

        if (IsGliding && !_pressingJump)
        {
            _rB.drag = linearDragRegular;
            // _animator.SetBool(AnimationStrings.isGliding, false);
            IsGliding = false;
            Logger.Log("AAAAA");
        }
    }


    private void StopGlideFromHit(GameObject player, int num)
    {
        if (player.CompareTag(TagStrings.playerTag))
        {
            _currentlyJumping = false;
            _coyoteTimeCounter = coyoteTime;
            _rB.drag = linearDragRegular;
            _animator.SetBool(AnimationStrings.isGliding, false);
            IsGliding = false;
            _inHit = true;
            _hitGlideDelayTimer = hitGlideDelayTime;
        }
    }
/*

timeToApexStat = scale(1, 10, 0.2f, 2.5f, numberFromPlatformerToolkit)


  public float scale(float OldMin, float OldMax, float NewMin, float NewMax, float OldValue)
    {

        float OldRange = (OldMax - OldMin);
        float NewRange = (NewMax - NewMin);
        float NewValue = (((OldValue - OldMin) * NewRange) / OldRange) + NewMin;

        return (NewValue);
    }

*/
}