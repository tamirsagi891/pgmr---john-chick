using System;
using UnityEngine;

namespace Mechanics.Enemies
{
    [AddComponentMenu("NPC/Attack Behaviours/Attack On Timer")]
    public class AttackOnTimer : AttackBehaviour
    {
        #region Public Properties

        public float Cooldown
        {
            get => _cooldown;
            set
            {
                _cooldown = value;
                _timePassed %= _cooldown;
            }
        }
        
        #endregion

        #region Private Fields

        private float _timePassed;
        private float _cooldown = 5f;

        #endregion

        #region MonoBehaviour

        protected override void OnEnable()
        {
            base.OnEnable();
            _timePassed = 0f;
        }

        protected void Update()
        {
            _timePassed += Time.deltaTime;
            while (_timePassed > Cooldown)
            {
                _timePassed -= Cooldown;
                if (npcToReportTo.CanAttack)
                {
                    npcToReportTo.Attack();
                }
            }
        }

        #endregion
        
    }
}
