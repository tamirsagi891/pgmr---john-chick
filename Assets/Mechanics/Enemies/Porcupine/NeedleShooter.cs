using System;
using Avrahamy;
using Avrahamy.Math;
using BitStrap;
using UnityEngine;
using static Mechanics.Enemies.Porcupine.ProjectileUtils;
using Logger = Nemesh.Logger;

namespace Mechanics.Enemies.Porcupine
{
    [AddComponentMenu("NPC/Attack Controls/Needle Shooter")]
    public class NeedleShooter : MonoBehaviour
    {
        #region Inspector
        
        [Header("Debug")]
        [SerializeField]
        [Tooltip("Should debug functions be used (rays, logs, etc)")]
        private bool debug;

        #endregion

        #region Private Fields

        private readonly PassiveTimer _burstTimer = new();
        private int _currentNeedle;
        private int _currentBurstSize;
        private AttackParameters _currentAttackParameters;
        private ProjectilePool _myPool;

        public ProjectilePool MyPool
        {
            get
            {
                if (_myPool == null)
                {
                    _myPool = ProjectilePoolManager.GetPool(ProjectilePoolType.Porcupine);
                }
                return _myPool;
            }
        }

        #endregion

        #region MonoBehaviour

        private void Update()
        {
            if (_currentNeedle >= _currentBurstSize)
            {
                return;
            }

            if (!_burstTimer.IsSet || _burstTimer.IsActive)
            {
                return;
            }

            SingleShotHandler();

        }

        #endregion

        #region Public Methods
        
        public void SingleShotHandler()
        {
            _burstTimer.Start();
            _currentNeedle += 1;
            ShotNeedle(_currentAttackParameters);
        }

        public void ShotNeedle(AttackParameters attackParameters)
        {
            Logger.LogWarning($"SHOULD IMPLEMENT SHOOTING NEEDLES." +
                              $" Needle {_currentNeedle} / {_currentBurstSize}." +
                              $"{_currentAttackParameters}",
                this);
            var shotDirection = attackParameters.Direction switch
            {
                Direction.Left => Vector3.left,
                Direction.Right => Vector3.right,
                _ => Vector3.right
            };
            if (debug)
            {
                Debug.DrawRay(transform.position, 
                    shotDirection.GetWithMagnitude(attackParameters.ShotSpeed * 2f), 
                    Color.red, 
                    _burstTimer.Duration / 2f);
            }

            var needle = MyPool.Pool.Get();
            needle.Parameters = attackParameters;
            needle.Shot(transform.position + shotDirection);
        }

        public void BurstAttack(int count, float time, AttackParameters attackParameters)
        {
            _currentNeedle = 0;
            _currentBurstSize = count;
            _burstTimer.Duration = time;
            _currentAttackParameters = attackParameters;

            if (count > 0)
            {
                SingleShotHandler();
            }
            if (debug)
            {
                Logger.Log($"Start Burst {count} : time={time}");
            }
        }

        #endregion

    }
}
