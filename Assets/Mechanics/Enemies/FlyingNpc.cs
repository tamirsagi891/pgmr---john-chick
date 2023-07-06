using System;
using Avrahamy.Math;
using BitStrap;
using UnityEngine;
using UnityEngine.Serialization;
using Logger = Nemesh.Logger;
using Random = UnityEngine.Random;

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
        private float minDistanceForMovement = 0.01f;

        [SerializeField]
        private bool addToDashLength;

        public ICanBeAttacked PickupTarget { get; set; }

        public Vector3 DesiredPosition { get; set; }
        
        protected override Transform WalkTargetHelper(Transform value)
        {
            value = base.WalkTargetHelper(value);
            if (!OnCd && HasPlayerContact && value != PlayerContact.GetTransform())  // $$ CanDetectPlayer
            {
                if (MovementBehaviour != null) // TODO: remove this on build
                {
                    MovementBehaviour.EnabledBehaviour = false;
                }
                Logger.Log("Why am I here?");
                value = PlayerContact.GetTransform();
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
        }

        public override void Dash()
        {
            base.Dash();
            DesiredPosition = WalkTarget.position;
            
            Vector2 directionToMove = DesiredPosition - transform.position;
            if (addToDashLength)
            {
                var v = directionToMove.normalized;
                DesiredPosition += new Vector3(v.x, v.y, 0f);
            }
            animationControls.SwitchDirection(directionToMove.x < 0 ? Direction.Left : Direction.Right);
            if (debug)
            {
                Debug.DrawLine(offsetPosition.position, DesiredPosition, Color.blue, 3);
            }
        }


        protected override void HandleMovementFixedUpdate()
        {
            if (animationControls.CanMove)
            {
                var shouldSwitch = ShouldSwitchDirectionWhenNotMoving();
                var targetPosition = IsDashing ? DesiredPosition : WalkTarget.position;
                Vector2 directionToMove = targetPosition - OffsetPosition.position;
                if (!IsDashing)
                {
                    animationControls.Direction = directionToMove.x < 0 ? Direction.Left : Direction.Right;
                }

                var minDistance = IsDashing ? minDistanceForMovementWhenHavePlayer : minDistanceForMovement;
                // Logger.Log(WalkTarget);
                if (HasPlayerContact)
                {
                    if (WalkTarget == PlayerContact.GetTransform())
                    {
                        if (directionToMove.sqrMagnitude < DashAlertDistance * DashAlertDistance)
                        {

                            if (HasDashControl && !IsDashing)
                            {
                                DashAlertControl.StartDashAlertSequence();
                            }
                        }
                    }
                    // else
                    // {
                    //     var distanceToPlayer = (PlayerContact.GetTransform().position - OffsetPosition.position);
                    //     if (distanceToPlayer.sqrMagnitude > DashAlertDistance * DashAlertDistance)
                    //     {
                    //         CanDetectPlayer = true;
                    //     }
                    // }

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
            else if (HasDefaultDirection)
            {
                animationControls.Direction = DefaultDirection;
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
            if (attackParameters.Type is AttackType.Pickup or AttackType.DashAndAway)
            {
                CanAttack = false;
                // CanDetectPlayer = false;
                PickupTarget = attackTarget;
                if (!succeeded)
                {
                    DropPickup();
                    return;
                }

                if (MovementBehaviour != null && attackParameters.Type is AttackType.Pickup) // TODO: remove this on build
                {
                    MovementBehaviour.EnabledBehaviour = false;
                }
            }
            events.onAttack.Invoke();
        }

        public void DropPickup()
        {
            // var oldTarget = PickupTarget;
            // if (PickupTarget == null)
            // {
            //     return;
            // }
            if (MyStatsHandler.CurrentStats.type is not (AttackType.Pickup or AttackType.DashAndAway))
            {
                return;
            }
            PickupTarget = null;
            CanAttack = true;
            AttackCdTimer.Start(MyStatsHandler.CurrentStats.cooldown);
            // CanDetectPlayer = false;

            if (MovementBehaviour != null)
            {
                MovementBehaviour.EnabledBehaviour = true;
                MovementBehaviour.GoToCurrentPoint();
            }

        }

        public override void StopDash()
        {
            if (!IsDashing)
            {
                return;
            }
            base.StopDash();
            if (AttackTargets.Count == 0)
            {
                DropPickup();
            }
        }
    }
}