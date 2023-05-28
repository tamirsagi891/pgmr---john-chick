using Avrahamy.Math;
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
    }
}