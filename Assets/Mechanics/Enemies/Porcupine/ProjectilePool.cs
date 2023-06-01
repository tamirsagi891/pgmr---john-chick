using System;
using BitStrap;
using UnityEngine;
using UnityEngine.Pool;

namespace Mechanics.Enemies.Porcupine
{
    public class ProjectilePool : MonoBehaviour
    {
        [SerializeField]
        [RequiredReference]
        private Projectile projectilePrefab;
        
        public LinkedPool<Projectile> MyPool { get; set; }
        private void Awake()
        {
            MyPool = new LinkedPool<Projectile>(
                InitProjectile, 
                GetProjectile,
                ReleaseProjectile,
                DestroyProjectile);
        }

        private void DestroyProjectile(Projectile projectile)
        {
            Destroy(projectile.gameObject);
        }

        private void ReleaseProjectile(Projectile projectile)
        {
            projectile.gameObject.SetActive(false);
        }

        private Projectile InitProjectile()
        {
            var projectile = Instantiate(projectilePrefab, transform);
            projectile.MyPool = this;
            return projectile;
        }

        private void GetProjectile(Projectile projectile)
        {
            projectile.gameObject.SetActive(true);
            projectile.InitObject();
        }
        
        
    }
}