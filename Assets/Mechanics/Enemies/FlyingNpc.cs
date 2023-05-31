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

        public ICanBeAttacked PickupTarget { get; set; }

        
        public override bool IsGrounded 
        {
            get => animationControls.IsGrounded && !animationControls.CanMove;
            set => animationControls.IsGrounded = value;
        }

        protected override void HandleMovementFixedUpdate()
        {
            if (animationControls.CanMove)
            {
                var shouldSwitch = ShouldSwitchDirectionWhenNotMoving();
                
                Vector2 directionToMove = WalkTarget.position - transform.position;

                animationControls.Direction = directionToMove.x < 0 ? Direction.Left : Direction.Right;
                var minDistance = 0.01f; // TODO: move both to fields
                if (HasPlayerContact)
                {
                    minDistance = minDistanceForMovementWhenHavePlayer;
                    // animationControls.StopDirectionSwitch = directionToMove.sqrMagnitude > minDistance;
                }
                ShouldMove = directionToMove.sqrMagnitude > minDistance;
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
            if (attackParameters.Type == AttackType.Pickup && succeeded)
            {
                CanAttack = false;
                PickupTarget = attackTarget;
                AttackTargets.Remove(attackTarget);
                WalkTarget = nestLocation;
                if (MovementBehaviour != null) // TODO: remove this on build
                {
                    MovementBehaviour.EnabledBehaviour = false;
                }
            
            }
            
            events.onAttack.Invoke();
        }


        public void DropPickup(ICanBeAttacked attackTarget)
        {
            if (PickupTarget == null)
            {
                return;
            }
            
            PickupTarget = null;
            CanAttack = true; 
            AttackCdTimer.Start(MyStatsHandler.CurrentStats.cooldown);
            if (MovementBehaviour != null) // TODO: remove this on build
            {
                MovementBehaviour.EnabledBehaviour = true;
                MovementBehaviour.GoToNextPoint();
            }
        }

    }
}