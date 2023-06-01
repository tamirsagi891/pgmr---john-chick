using Avrahamy;
using Nemesh;
using UnityEngine;

namespace Mechanics.Enemies.Porcupine
{
    [AddComponentMenu("NPC/Attack Controls/Base Projectile")]
    public class Projectile : OptimizedBehaviour, IPoolable
    {
        public ProjectilePool MyPool { get; set; }
        
        public void ReleaseSelf()
        {
            throw new System.NotImplementedException();
        }

        public void InitObject()
        {
            throw new System.NotImplementedException();
        }
    }
}