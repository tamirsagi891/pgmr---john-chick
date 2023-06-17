using UnityEngine;
using UnityEngine.Events;

namespace Elad.Scripts.Events
{
    public static class ParticleEvents
    {
        public static UnityEvent PlayerDust = new();
        public static UnityEvent PlayerJump = new();
        public static UnityEvent<bool> PlayerGlide = new();

        
    }
}
