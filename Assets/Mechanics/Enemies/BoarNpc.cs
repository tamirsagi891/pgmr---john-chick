using UnityEngine;

namespace Mechanics.Enemies
{
    [AddComponentMenu("NPC/Boar")]
    public class BoarNpc : BaseNpc
    {
        [Header("Boar")]
        [SerializeField]
        private bool resetCooldownOnDashEnd = true;
        
        public override ICanBeAttacked PlayerContact
        {
            get => base.PlayerContact;
            set
            {
                base.PlayerContact = value;
                if (CanDashAttack)
                {
                    DashAlertControl.StartDashAlertSequence();
                }
            }
        }

        private bool CanDashAttack => HasPlayerContact && HasDashControl && !IsDashing && IsGrounded && CanAttack &&
                                      !(AttackCdTimer.IsSet && AttackCdTimer.IsActive);

        public override void StopDash()
        {
            base.StopDash();
            if (resetCooldownOnDashEnd)
            {
                AttackCdTimer.Clear();
            }
        }
    }
}
