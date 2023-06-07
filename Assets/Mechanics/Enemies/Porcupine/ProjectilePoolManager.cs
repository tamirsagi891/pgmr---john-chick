using System;
using System.Collections.Generic;
using Nemesh;
using static Mechanics.Enemies.Porcupine.ProjectileUtils;

namespace Mechanics.Enemies.Porcupine
{
    public static class ProjectilePoolManager
    {
        private static readonly Dictionary<ProjectilePoolType, ProjectilePool> Pools = new();

        public static ProjectilePool GetPool(ProjectilePoolType type)
        {
            return Pools.TryGetValue(type, out var pool) ? pool : null;
        }

        public static bool SetPool(ProjectilePoolType type, ProjectilePool pool)
        {
            return Pools.TryAdd(type, pool);
        }

        public static bool RemovePool(ProjectilePoolType type, ProjectilePool projectilePool)
        {
            return Pools.ContainsValue(projectilePool) && Pools.Remove(type);
        }
    }
}
