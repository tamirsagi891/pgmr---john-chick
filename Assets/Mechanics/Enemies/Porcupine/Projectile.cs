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
        [Header("Base Projectile")]
        [SerializeField]
        protected UnityEvent<Projectile> onRelease;
        
        [SerializeField]
        protected UnityEvent onAttack;

        [SerializeField]
        [Min(0)]
        protected float releaseAfterTime;
        
        public AttackParameters Parameters { get; set; }
        public ProjectilePool MyProjectilePool { get; set; }

        protected float ReleaseAfterTime
        {
            get => releaseAfterTime <= 0 ? Time.fixedDeltaTime : releaseAfterTime;
            set => releaseAfterTime = value <= 0 ? Time.fixedDeltaTime : value;
        }

        public virtual void ReleaseSelf()
        {
            onRelease.Invoke(this);
            MyProjectilePool.Pool.Release(this);
        }

        public virtual void InitObject()
        {
        }

        public virtual void Shot(Vector3 position)
        {
            transform.position = position;
            SetDirection();

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

        public virtual bool Attack(ICanBeAttacked attackTarget)
        {
            var succeeded = attackTarget.Hurt(Parameters);
            onAttack.Invoke();
            StartCoroutine(DelayExecution(ReleaseAfterTime, ReleaseSelf));
            return succeeded;
        }

        public AttackParameters GetAttackParameters()
        {
            return Parameters ?? new AttackParameters(this);
        }
    }

}