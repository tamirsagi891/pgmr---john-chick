using System;
using Avrahamy.Math;
using BitStrap;
using UnityEngine;
using UnityEngine.Serialization;
using Logger = Nemesh.Logger;

namespace Mechanics.Enemies
{
    [AddComponentMenu("NPC/Flying Npc")]
    public class FlyingNpc : BaseNpc
    {
        [Space(2)]
        [Header("Flying Npc")]
        [SerializeField]
        private float velocitySmoothTime = 0.5f;

        [SerializeField]
        [RequiredReference]
        private Transform nestLocation;

        private float _dashAlertDistance = 7f;
        private DashAndAlertControl _dashAlertControl;
        private bool _hasDashControl;


        public ICanBeAttacked PickupTarget { get; set; }
        public Vector3 DesiredPosition { get; set; }

        protected override Transform WalkTargetHelper(Transform value)
        {
            if (HasPlayerContact && _walkTarget != PlayerContact.GetTransform())
            {
                canDetectPlayer = true;
            }

            if (HasPlayerContact && canDetectPlayer)
            {
                value = PlayerContact.GetTransform();
                if (MovementBehaviour != null) // TODO: remove this on build
                {
                    MovementBehaviour.EnabledBehaviour = false;
                }
            }

            return value;
        }

        public override bool IsGrounded
        {
            get => animationControls.IsGrounded && !animationControls.CanMove;
            set => animationControls.IsGrounded = value;
        }

        protected override void Awake()
        {
            base.Awake();
            events.onDashEnd.AddListener(DropPickup);
            _dashAlertControl = GetComponentInChildren<DashAndAlertControl>();
            _hasDashControl = _dashAlertControl != null;
        }

        protected virtual void OnEnable()
        {
            if (_hasDashControl)
            {
                _dashAlertDistance = _dashAlertControl.Radius;
            }
        }

        public override void Dash()
        {
            base.Dash();
            DesiredPosition = WalkTarget.position;
        }


        protected override void HandleMovementFixedUpdate()
        {
            if (animationControls.CanMove)
            {
                var shouldSwitch = ShouldSwitchDirectionWhenNotMoving();
                var targetPosition = IsDashing ? DesiredPosition : WalkTarget.position;
                Vector2 directionToMove = targetPosition - transform.position;
                if (!IsDashing)
                {
                    animationControls.Direction = directionToMove.x < 0 ? Direction.Left : Direction.Right;
                }

                var minDistance = 0.01f; // TODO: move both to fields
                // Logger.Log(WalkTarget);
                if (HasPlayerContact && WalkTarget == PlayerContact.GetTransform())
                {
                    // TODO: calculate in validate
                    if (directionToMove.sqrMagnitude < _dashAlertDistance * _dashAlertDistance)
                    {
                        if (_hasDashControl && !IsDashing)
                        {
                            _dashAlertControl.StartDashAlertSequence();
                        }
                    }

                    minDistance = minDistanceForMovementWhenHavePlayer;
                    // animationControls.StopDirectionSwitch = directionToMove.sqrMagnitude > minDistance;
                }

                ShouldMove = directionToMove.sqrMagnitude > minDistance;
                if (!ShouldMove && IsDashing)
                {
                    StopDash();
                }
                DesiredVelocity = ShouldMove
                    ? directionToMove.GetWithMagnitude(MyStatsHandler.CurrentStats.movementSpeed)
                    : Vector2.zero;

                if (shouldSwitch) // TODO: ignore edges in pursuit?
                {
                    HandleDirectionSwitch();
                }

                RunWithoutAcceleration();
            }
            else if (hasDefaultDirection)
            {
                animationControls.Direction = defaultDirection;
            }
        }

        protected override void RunWithoutAcceleration()
        {
            var newVelocity = Vector2.SmoothDamp(MyRigidbody.velocity, DesiredVelocity,
                ref Velocity, velocitySmoothTime, MyStatsHandler.CurrentStats.movementSpeed, Time.fixedDeltaTime);
            MyRigidbody.velocity = newVelocity;
        }


        protected override void AttackTargetUsingParams(ICanBeAttacked attackTarget, AttackParameters attackParameters)
        {
            var succeeded = attackTarget.Hurt(attackParameters);
            if (attackParameters.Type == AttackType.Pickup)
            {
                CanAttack = false;
                PickupTarget = attackTarget;
                WalkTarget = nestLocation;
                if (!succeeded)
                {
                    DropPickup();
                    return;
                }

                if (MovementBehaviour != null) // TODO: remove this on build
                {
                    MovementBehaviour.EnabledBehaviour = false;
                }

                events.onAttack.Invoke();
            }
        }

        public void DropPickup()
        {
            if (PickupTarget == null)
            {
                return;
            }

            PickupTarget = null;
            CanAttack = true;
            AttackCdTimer.Start(MyStatsHandler.CurrentStats.cooldown);
            CanDetectPlayer = false;

            if (MovementBehaviour != null)
            {
                MovementBehaviour.EnabledBehaviour = true;
                MovementBehaviour.GoToNextPoint();
                return;
            }

            WalkTarget = nestLocation;
        }
    }
}