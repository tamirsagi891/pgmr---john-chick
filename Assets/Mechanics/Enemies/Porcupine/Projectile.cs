using System;
using Avrahamy;
using Nemesh;
using UnityEngine;
using UnityEngine.Events;
using static Mechanics.Enemies.CorotuineUtils;
using static Mechanics.Enemies.Porcupine.ProjectileUtils;
using Logger = Nemesh.Logger;

namespace Mechanics.Enemies.Porcupine
{
    [AddComponentMenu("NPC/Attack Controls/Base Projectile")]
    public class Projectile : OptimizedBehaviour, IPoolable, IAttacker
    {

        #region Inspector

        [Header("Base Projectile")]
        [SerializeField]
        protected UnityEvent<Projectile> onRelease;
        
        [SerializeField]
        protected UnityEvent onAttack;

        [Space]
        [SerializeField]
        protected PassiveTimer endLifeAfterTime = new(10f);
        
        [SerializeField]
        [Min(0)]
        protected float releaseAfterTime;

        #endregion

        #region Properties

        public AttackParameters Parameters { get; set; }
        public ProjectilePool MyProjectilePool { get; set; }

        protected float ReleaseAfterTime
        {
            get => releaseAfterTime <= 0 ? Time.fixedDeltaTime : releaseAfterTime;
            set => releaseAfterTime = value <= 0 ? Time.fixedDeltaTime : value;
        }

        #endregion

        #region MonoBehaviour

        protected virtual void Update()
        {
            if (endLifeAfterTime.IsSet && !endLifeAfterTime.IsActive)
            {
                ReleaseSelf();
            }
        }

        #endregion

        #region IPoolable

        public virtual void ReleaseSelf()
        {
            endLifeAfterTime.Clear();
            onRelease.Invoke(this);
            MyProjectilePool.Pool.Release(this);
        }

        public virtual void InitObject()
        {
        }

        #endregion

        #region Projectile

        public virtual void Shot(Vector3 position)
        {
            transform.position = position;
            SetDirection();
            StartLifetime();
        }

        protected virtual void StartLifetime()
        {
            endLifeAfterTime.Start();
        }

        protected virtual void SetDirection()
        {
            var scaleFactor = Parameters.Direction switch
            {
                Direction.Left => ScaleLeft,
                Direction.Right => ScaleRight,
                _ => ScaleRight
            };
            transform.localScale = Vector3.Scale(transform.localScale, scaleFactor);
        }

        #endregion

        #region IAttacker

        public virtual bool Attack(ICanBeAttacked attackTarget)
        {
            var succeeded = attackTarget.Hurt(Parameters);
            // TODO: only if succeeded
            endLifeAfterTime.Clear();
            onAttack.Invoke();
            StartCoroutine(DelayExecution(ReleaseAfterTime, ReleaseSelf));
            return succeeded;
        }

        public AttackParameters GetAttackParameters()
        {
            return Parameters ?? new AttackParameters(this);
        }

        #endregion
    }

}