using UnityEngine;
using Logger = Nemesh.Logger;

namespace Mechanics.Enemies
{
    [AddComponentMenu("NPC/Boar")]
    public class BoarNpc : BaseNpc
    {
        [Header("Boar")]
        [SerializeField]
        private bool resetCooldownOnDashEnd = true;

        private bool CanDashAttack => HasPlayerContact && HasDashControl && !IsDashing && IsGrounded && CanAttack &&
                                      !(AttackCdTimer.IsSet && AttackCdTimer.IsActive);

        protected override void Update()
        {
            if (CanDashAttack && !DashAlertControl.WaitingForDash)
            {
                DashAlertControl.StartDashAlertSequence();
            }

            base.Update();
        }

        public override void StopDash()
        {
            base.StopDash();
            if (resetCooldownOnDashEnd)
            {
                AttackCdTimer.Clear();
            }

            if (debug)
            {
                Logger.Log("Dash End", this);
            }
        }
    }
}