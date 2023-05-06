using System;
using Avrahamy;
using BitStrap;
using UnityEngine;

namespace Mechanics.Enemies
{
    [AddComponentMenu("NPC/Animation Controls")]
    [RequireComponent(typeof(Animator))]
    public class NpcAnimationControls : MonoBehaviour
    {

        #region Inspector

        #region Animator State

        [Header("Timers - TODO: ADD TRIGGER TIMERS HERE TOO!")]
        [SerializeField]
        private PassiveTimer edgeChangeTimer = new(0.5f);

        [Header("Animation State")]
        [HelpBox("The parameters are updated onValidate to allow control " +
                 "from the Inspector. From code all updates can be done with the" +
                 " corresponding public Properties.",
            HelpBoxAttribute.MessageType.Info)]
        [SerializeField]
        private bool canMove = true;

        [SerializeField]
        private bool isMoving;

        [SerializeField]
        private bool isRunning;

        [SerializeField]
        private bool isGrounded;

        [SerializeField]
        private bool isDead;

        [Space]
        [SerializeField]
        private bool jump;

        [SerializeField]
        private bool dash;

        [SerializeField]
        private bool attack;

        [SerializeField]
        private bool hurt;

        [Space]
        [SerializeField]
        private float yVelocity;

        [SerializeField]
        private Direction direction = Direction.Right;

        [Space]
        [SerializeField]
        private bool doubleJump;

        [SerializeField]
        private bool dodgeRoll;

        [Space]
        [SerializeField]
        private bool isCrouching;

        [SerializeField]
        private bool isGliding;

        [SerializeField]
        private bool isWallSliding;

        [SerializeField]
        private bool isOnWall;

        [SerializeField]
        private bool isOnCeiling;

        #endregion

        [Space]
        [SerializeField]
        private MovementAnimatorParameters animatorParameters;

        [Space]
        [SerializeField]
        private Animator myAnimator;

        #endregion
        
        #region Animator Properties

        #region Transform

        public Direction Direction
        {
            get => _currentDirection;
            set
            {
                if (value == _currentDirection)
                {
                    return;
                }

                if (edgeChangeTimer.IsSet && edgeChangeTimer.IsActive)
                {
                    return;
                }

                SwitchDirection(value);
            }
        }

        #endregion

        #region Booleans

        public bool CanMove
        {
            get => canMove;
            set
            {
                canMove = value; // TODO: this is not used now in the controller!
                animatorParameters.canMove.Set(myAnimator, canMove);
            }
        }

        public bool IsMoving
        {
            get => isMoving;
            set
            {
                isMoving = value;
                animatorParameters.isMoving.Set(myAnimator, isMoving);
            }
        }

        public bool IsRunning
        {
            get => isRunning;
            set
            {
                isRunning = value;
                animatorParameters.isRunning.Set(myAnimator, isRunning);
            }
        }

        public bool IsGrounded
        {
            get => isGrounded;
            set
            {
                isGrounded = value;
                animatorParameters.isGrounded.Set(myAnimator, isGrounded);
            }
        }

        public bool IsDead
        {
            get => isDead;
            set
            {
                isDead = value;
                animatorParameters.isDead.Set(myAnimator, isDead);
            }
        }

        public bool IsCrouching
        {
            get => isCrouching;
            set
            {
                isCrouching = value;
                animatorParameters.isCrouching.Set(myAnimator, isCrouching);
            }
        }

        public bool IsGliding
        {
            get => isGliding;
            set
            {
                isGliding = value;
                animatorParameters.isGliding.Set(myAnimator, isGliding);
            }
        }

        public bool IsWallSliding
        {
            get => isWallSliding;
            set
            {
                isWallSliding = value;
                animatorParameters.isWallSliding.Set(myAnimator, isWallSliding);
            }
        }

        public bool IsOnWall
        {
            get => isOnWall;
            set
            {
                isOnWall = value;
                animatorParameters.isOnWall.Set(myAnimator, isOnWall);
            }
        }

        public bool IsOnCeiling
        {
            get => isOnCeiling;
            set
            {
                isOnCeiling = value;
                animatorParameters.isOnCeiling.Set(myAnimator, isOnCeiling);
            }
        }

        #endregion

        #region Triggers

        public bool Jump
        {
            get => jump;
            set
            {
                if (value)
                {
                    animatorParameters.jump.Set(myAnimator);
                }

                jump = false;
            }
        }

        public bool Dash
        {
            get => dash;
            set
            {
                if (value)
                {
                    animatorParameters.dash.Set(myAnimator);
                }

                dash = false;
            }
        }

        public bool Attack
        {
            get => attack;
            set
            {
                if (value)
                {
                    animatorParameters.attack.Set(myAnimator);
                }

                attack = false;
            }
        }

        public bool Hurt
        {
            get => hurt;
            set
            {
                if (value)
                {
                    animatorParameters.hurt.Set(myAnimator);
                }

                hurt = false;
            }
        }

        public bool DoubleJump
        {
            get => doubleJump;
            set
            {
                if (value)
                {
                    animatorParameters.doubleJump.Set(myAnimator);
                }

                doubleJump = false;
            }
        }

        public bool DodgeRoll
        {
            get => dodgeRoll;
            set
            {
                if (value)
                {
                    animatorParameters.dodgeRoll.Set(myAnimator);
                }

                dodgeRoll = false;
            }
        }

        #endregion

        #region Floats

        public float YVelocity
        {
            get => yVelocity;
            set => yVelocity = value;
        }

        #endregion

        #endregion
        
        #region Private Fields

        private Direction _currentDirection;

        #endregion

        #region MonoBehaviour

        private void Awake()
        {
            if (myAnimator == null)
            {
                myAnimator = GetComponent<Animator>();
            }

            HandleTriggers();
            HandleBooleans();
            _currentDirection = direction == Direction.Left ? Direction.Right : Direction.Left;
            SwitchDirection(direction, true);
        }

        private void OnValidate()
        {
            HandleTriggers();
            HandleBooleans();
            SwitchDirection(direction, true);
        }

        private void FixedUpdate()
        {
            HandleFloatAnimations();
        }

        #endregion

        #region Public Methods

        public void SwitchDirection()
        {
            var value = _currentDirection switch
            {
                Direction.Left => Direction.Right,
                Direction.Right => Direction.Left,
                _ => throw new ArgumentOutOfRangeException()
            };
            SwitchDirection(value);
        }

        public void SwitchDirection(Direction value, bool validate = false)
        {
            if (!validate) // TODO: remove in build
            {
                edgeChangeTimer.Start();
            }

            var currentScale = transform.localScale;
            var xScale = Mathf.Abs(currentScale.x);
            transform.localScale = new Vector3(
                value == Direction.Left ? -xScale : xScale,
                currentScale.y,
                currentScale.z
            );
            _currentDirection = value;
            direction = value;
        }

        #endregion

        #region Parameter Handlers

        private void HandleBooleans()
        {
            CanMove = canMove;
            IsMoving = isMoving;
            IsRunning = isRunning;
            IsGrounded = isGrounded;
            IsDead = isDead;
            IsCrouching = isCrouching;
            IsGliding = isGliding;
            IsWallSliding = isWallSliding;
            IsOnWall = isOnWall;
            IsOnCeiling = isOnCeiling;
        }

        private void HandleTriggers()
        {
            Jump = jump;
            Dash = dash;
            Attack = attack;
            Hurt = hurt;
            DoubleJump = doubleJump;
            DodgeRoll = dodgeRoll;
        }


        private void HandleFloatAnimations()
        {
            animatorParameters.yVelocity.Set(myAnimator, YVelocity);
        }

        #endregion

    }


    /// <summary>
    ///     This struct can be used to define animator parameters for attached animator component for movement
    /// </summary>
    [Serializable]
    public struct MovementAnimatorParameters
    {
        [SerializeField]
        public BoolAnimationParameter canMove;

        [SerializeField]
        public BoolAnimationParameter isMoving;

        [SerializeField]
        public BoolAnimationParameter isRunning;

        [SerializeField]
        public BoolAnimationParameter isGrounded;

        [SerializeField]
        public BoolAnimationParameter isDead;

        [Space]
        [SerializeField]
        public TriggerAnimationParameter jump;

        [SerializeField]
        public TriggerAnimationParameter dash;

        [SerializeField]
        public TriggerAnimationParameter attack;

        [SerializeField]
        public TriggerAnimationParameter hurt;

        [Space]
        [SerializeField]
        public FloatAnimationParameter yVelocity;

        [Space]
        [SerializeField]
        public TriggerAnimationParameter doubleJump;

        [SerializeField]
        public TriggerAnimationParameter dodgeRoll;

        [Space]
        [SerializeField]
        public BoolAnimationParameter isCrouching;

        [SerializeField]
        public BoolAnimationParameter isGliding;

        [SerializeField]
        public BoolAnimationParameter isWallSliding;

        [SerializeField]
        public BoolAnimationParameter isOnWall;

        [SerializeField]
        public BoolAnimationParameter isOnCeiling;
    }

    [Serializable]
    public enum Direction
    {
        Left,
        Right
    }
}
