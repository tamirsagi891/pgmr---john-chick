using System;
using BitStrap;
using UnityEngine;
using UnityEngine.Pool;
using static Mechanics.Enemies.Porcupine.ProjectileUtils;
using Logger = Nemesh.Logger;

namespace Mechanics.Enemies.Porcupine
{
    public class ProjectilePool : MonoBehaviour
    {

        [SerializeField]
        [ReadOnly(onlyInPlaymode = true)]
        protected ProjectilePoolType poolFor = ProjectilePoolType.Porcupine;

        [SerializeField]
        [RequiredReference]
        protected Projectile projectilePrefab;

        public LinkedPool<Projectile> Pool { get; set; }

        protected void Awake()
        {
            var instance = ProjectilePoolManager.GetPool(poolFor);
            if (instance != null)
            {
                Logger.LogWarning($"Pool of type {poolFor} already exists! ({instance.name})", gameObject);
                Destroy(this);
                return;
            }

            Pool = new LinkedPool<Projectile>(
                InitProjectile,
                GetProjectile,
                ReleaseProjectile,
                DestroyProjectile);
            ProjectilePoolManager.SetPool(poolFor, this);
        }

        private void OnDestroy()
        {
            ProjectilePoolManager.RemovePool(poolFor, this);
        }

        protected void DestroyProjectile(Projectile projectile)
        {
            Destroy(projectile.gameObject);
        }

        protected void ReleaseProjectile(Projectile projectile)
        {
            projectile.gameObject.SetActive(false);
        }

        protected Projectile InitProjectile()
        {
            var projectile = Instantiate(projectilePrefab, transform);
            projectile.MyProjectilePool = this;
            return projectile;
        }

        protected void GetProjectile(Projectile projectile)
        {
            projectile.gameObject.SetActive(true);
            projectile.InitObject();
        }
    }
}
