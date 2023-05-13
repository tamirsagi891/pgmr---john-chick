using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

//This script handles moving the character on the Y axis, for jumping and gravity

public class CharacterJump : MonoBehaviour
{
    [Header("Components")] private Rigidbody2D _rB;

    private WallMovement _wallMovement;
    private TouchingDirection _touchingDirection;
    private Vector2 _velocity;

    private PlayerController _playerController;
    private Animator _animator;


    [Header("Jumping Stats")] [SerializeField, Range(2f, 5.5f)]
    private float maxJumpHeight = 7.3f;

    [SerializeField, Range(0.2f, 1.25f)] private float timeToReachPeakHeight;

    [SerializeField, Range(0f, 5f)] [Tooltip("Gravity multiplier to apply when going up")]
    private float gravityMultiplierAscending = 1f;

    [SerializeField, Range(1f, 10f)] [Tooltip("Gravity multiplier to apply when coming down")]
    private float gravityMultiplierDescending = 6.17f;

    [Header("Options")] [SerializeField] private bool canDoubleJump;
    [SerializeField] private bool dropWhenStopPushingJump;

    [SerializeField, Range(1f, 10f)] private float gravityPercentLetGoJump;

    [SerializeField] [Tooltip("The fastest speed the character can fall")]
    private float maxFallSpeed;

    [SerializeField, Range(0f, 0.3f)] [Tooltip("How long should coyote time last?")]
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
    private float _coyoteTimeCounter = 0;
    private bool _pressingJump;
    private bool onGround;
    private bool _currentlyJumping;

    [Header("Gliding")] [SerializeField, Range(0f, 5f)] [Tooltip("Gravity multiplier to apply when gliding")]
    private float gravityMultiplierGliding = 0.3f;

    [SerializeField, Range(0f, 5f)] [Tooltip("linear Drag to apply when gliding")]
    private float linearDragGliding = 2f;

    private float linearDragRegular;

    private bool _canGlide = true;
    private bool _isGliding;

    private bool CanGlide
    {
        get
        {
            var returnValue = !(_touchingDirection.IsGrounded);
            return returnValue;
        }
    }


    [Header("Wall Sliding")] [SerializeField, Range(0f, 5f)] [Tooltip("Gravity multiplier to apply when sliding")]
    private float gravityMultiplierWallSliding = 0.3f;

    


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
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        //This function is called when one of the jump buttons (like space or the A button) is pressed.
        if (_playerController.CanMove)
        {
            //When we press the jump button, tell the script that we desire a jump.
            //Also, use the started and canceled contexts to know if we're currently holding the button
            if (context.started)
            {
                _desiredJump = true;
                _pressingJump = true;
            }

            if (context.canceled)
            {
                _pressingJump = false;
            }
        }
    }


    void Update()
    {
        setPhysics();

        //Check if we're on ground.
        onGround = _touchingDirection.IsGrounded;

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
        if (!_currentlyJumping && !onGround)
        {
            _coyoteTimeCounter += Time.deltaTime;
        }
        else
        {
            //Reset it when we touch the ground, or jump
            _coyoteTimeCounter = 0;
        }
    }

    private void setPhysics()
    {
        //Determine the character's gravity scale, using the stats provided. Multiply it by a _gravMultiplier, used later
        Vector2 newGravity = new Vector2(0, (-2 * maxJumpHeight) / (timeToReachPeakHeight * timeToReachPeakHeight));
        _rB.gravityScale = (newGravity.y / Physics2D.gravity.y) * _gravMultiplier;
    }

    private void FixedUpdate()
    {
        //Get _velocity from Kit's Rigidbody 
        _velocity = _rB.velocity;

        //Keep trying to do a jump, for as long as _desiredJump is true
        if (_desiredJump)
        {
            DoAJump();
            _rB.velocity = _velocity;

            //Skip gravity calculations this frame, so _currentlyJumping doesn't turn off
            //This makes sure you can't do the coyote time double jump bug
            return;
        }

        calculateGravity();
        _animator.SetFloat(AnimationStrings.yVelocity, _rB.velocity.y);
    }

    private void calculateGravity()
    {
        //We change the character's gravity based on her Y direction

        //If Kit is going up...
        if (_rB.velocity.y > 0.01f)
        {
            if (onGround)
            {
                //Don't change it if Kit is stood on something (such as a moving platform)
                _gravMultiplier = _defaultGravityScale;
            }
            else
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
        }

        //Else if going down...
        else if (_rB.velocity.y < -0.01f)
        {
            if (onGround)
                //Don't change it if Kit is stood on something (such as a moving platform)
            {
                _gravMultiplier = _defaultGravityScale;
            }
            else if (_wallMovement.IsWallSliding)
            {
                _gravMultiplier = gravityMultiplierWallSliding;
            }
            
            else if (_isGliding)
            {
                _gravMultiplier = gravityMultiplierGliding;
            }

            else
            {
                //Otherwise, apply the downward gravity multiplier as Kit comes back to Earth
                _gravMultiplier = gravityMultiplierDescending;
            }
        }
        //Else not moving vertically at all
        else
        {
            if (onGround)
            {
                _currentlyJumping = false;
            }

            _gravMultiplier = _defaultGravityScale;
        }

        //Set the character's Rigidbody's _velocity
        //But clamp the Y variable within the bounds of the speed limit, for the terminal _velocity assist option
        _rB.velocity = new Vector3(_velocity.x, Mathf.Clamp(_velocity.y, -maxFallSpeed, 100));
    }

    private void DoAJump()
    {
        // Create the jump, provided we are on the ground, in coyote time, or have a double jump available
        if (onGround || (_coyoteTimeCounter > 0.03f && _coyoteTimeCounter < coyoteTime) || canJumpAgain || _wallMovement.IsWallSliding)
        {
            _desiredJump = false;
            _jumpBufferCounter = 0;
            _coyoteTimeCounter = 0;

            // If we have double jump on, allow us to jump again (but only once)
            bool isInDoubleJump = !(canDoubleJump && (canJumpAgain == false));
            canJumpAgain = !isInDoubleJump;

            // Determine the power of the jump, based on our gravity and stats
            _jumpSpeed = Mathf.Sqrt(-2f * Physics2D.gravity.y * _rB.gravityScale * maxJumpHeight);

            // Apply double jump multiplier if it's a double jump
            if (isInDoubleJump)
            {
                _jumpSpeed *= doubleJumpMultiplier;
            }

            // If Kit is moving up or down when she jumps (such as when doing a double jump), change the _jumpSpeed;
            // This will ensure the jump is the exact same strength, no matter your _velocity.
            if (_velocity.y > 0f)
            {
                _jumpSpeed = Mathf.Max(_jumpSpeed - _velocity.y, 0f);
            }
            else if (_velocity.y < 0f)
            {
                _jumpSpeed += Mathf.Abs(_rB.velocity.y);
            }

            // Apply the new _jumpSpeed to the _velocity. It will be sent to the Rigidbody in FixedUpdate;
            var whichJumpAnimation = AnimationStrings.jumpTrigger;
            if (!(_touchingDirection.IsGrounded))
            {
                whichJumpAnimation = AnimationStrings.doubleJumpTrigger;
            }

            _animator.SetTrigger(whichJumpAnimation);

            _velocity.y += _jumpSpeed;
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

    public void OnGlide(InputAction.CallbackContext context)
    {
        if (context.started && CanGlide && !_isGliding)
        {
            _rB.drag = linearDragGliding;
            _animator.SetBool(AnimationStrings.isGliding, true);
            _isGliding = true;
        }

        if (context.canceled && _isGliding)
        {
            _rB.drag = linearDragRegular;
            _animator.SetBool(AnimationStrings.isGliding, false);
            _isGliding = false;
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