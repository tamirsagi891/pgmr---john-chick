using System;
using System.Collections.Generic;
using UnityEngine;
using Logger = Nemesh.Logger;

namespace Mechanics.New_Wind
{
    [AddComponentMenu("Wind/Utils/Wind Particle Trigger Callback")]
    [RequireComponent(typeof(ParticleSystem))]
    public class WindParticleCallbackHandler : MonoBehaviour
    {
        [SerializeField]
        private float lifeAtExit = 0.5f;

        private ParticleSystem _myParticleSystem;
        private readonly List<ParticleSystem.Particle> _exit = new();
        private bool _hasSystem;

        private void OnValidate()
        {
            _hasSystem = TryGetComponent(out _myParticleSystem);
        }

        // Start is called before the first frame update
        void OnEnable()
        {
            _hasSystem = TryGetComponent(out _myParticleSystem);
        }

        private void OnParticleTrigger()
        {
            if (!_hasSystem)
            {
                return;
            }
            
            var numExit = _myParticleSystem.GetTriggerParticles(ParticleSystemTriggerEventType.Exit, _exit);

            for (var i = 0; i < numExit; i++)
            {
                var p = _exit[i];
                p.remainingLifetime = lifeAtExit;
                _exit[i] = p;
            }
            
            _myParticleSystem.SetTriggerParticles(ParticleSystemTriggerEventType.Exit, _exit);
        }
    }
}
