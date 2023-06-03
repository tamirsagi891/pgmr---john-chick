using System;
using UnityEngine;

namespace Mechanics.Enemies.Porcupine
{
    public static class ProjectileUtils
    {
        [Serializable]
        public enum ProjectilePoolType
        {
            Porcupine
        }
        
        #region Public Static

        public static readonly Vector3 ScaleLeft = new(-1, 1, 1);
        public static readonly Vector3 ScaleRight = new(-1, 1, 1);

        #endregion
    }
}
