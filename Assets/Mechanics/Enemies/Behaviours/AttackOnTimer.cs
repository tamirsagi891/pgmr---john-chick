using System;
using Avrahamy;
using Avrahamy.EditorGadgets;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Mechanics.Enemies
{
    [AddComponentMenu("NPC/Attack Behaviours/Attack On Timer")]
    public class AttackOnTimer : AttackBehaviour
    {

        #region Inspector

        [SerializeField]
        private bool randomizeStartTime = true;

        [ConditionalHide("randomizeStartTime", true, true)]
        [SerializeField]
        private float startDelay = 1f;

        #endregion
        
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
        private bool _firstFrame = true;
        private PassiveTimer _startDelayTimer;

        #endregion

        #region MonoBehaviour

        protected override void OnEnable()
        {
            startDelay = randomizeStartTime ? Random.value : startDelay;
            _startDelayTimer = new PassiveTimer(startDelay);
            _startDelayTimer.Start();
            base.OnEnable();
            _timePassed = 0f;
        }

        protected void Update()
        {
            if (_firstFrame)
            {
                if (DelayTimingHandler())
                {
                    return;
                }
            }
            
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

        #region Private Methods

        private bool DelayTimingHandler()
        {
            if (_startDelayTimer.IsSet && _startDelayTimer.IsActive)
            {
                return true;
            }

            _firstFrame = false;
            return false;
        }

        #endregion
    }
}
