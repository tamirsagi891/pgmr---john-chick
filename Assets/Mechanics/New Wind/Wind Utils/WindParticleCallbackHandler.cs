using System;
using System.Collections.Generic;
using BitStrap;
using UnityEngine;
using Logger = Nemesh.Logger;

namespace Mechanics.New_Wind
{
    [AddComponentMenu("Wind/Utils/Wind Particle Trigger Callback")]
    [RequireComponent(typeof(ParticleSystem))]
    public class WindParticleCallbackHandler : MonoBehaviour
    {
        [SerializeField]
        private bool killImmediate;
        
        [SerializeField]
        private float lifeAtExit = 0.5f;

        public bool KillImmediate
        {
            get => killImmediate;
            set => killImmediate = value;
        }

        private ParticleSystem _myParticleSystem;
        private readonly List<ParticleSystem.Particle> _exit = new();
        private bool _hasSystem;

        #region MonoBehaviour

        private void OnValidate()
        {
            _hasSystem = TryGetComponent(out _myParticleSystem);
            // if (_hasSystem)
            // {
            //     _pauseParticles = new ParticleSystem.Particle[_myParticleSystem.main.maxParticles];
            // }
        }

        // Start is called before the first frame update
        void OnEnable()
        {
            _hasSystem = TryGetComponent(out _myParticleSystem);
            // if (_hasSystem)
            // {
            //     _pauseParticles = new ParticleSystem.Particle[_myParticleSystem.main.maxParticles];
            // }
        }

        #endregion

        #region Public Methods

        [Button]
        public void PauseParticles()
        {
            if (_hasSystem && _myParticleSystem.isEmitting)
            {
                var em = _myParticleSystem.emission;
                em.enabled = false;
                // _myParticleSystem.Stop(true, behaviourOnPause);
                // if (behaviourOnPause == ParticleSystemStopBehavior.StopEmitting)
                // {
                //     var numExit = _myParticleSystem.GetParticles(_pauseParticles);
                //     for (var i = 0; i < numExit; i++)
                //     {
                //         var p = _pauseParticles[i];
                //         p.remainingLifetime = lifeAtExit;
                //         _pauseParticles[i] = p;
                //     }
                //     _myParticleSystem.SetParticles(_pauseParticles, numExit);
                // }
                // else
                // {
                //     _myParticleSystem.Clear(true);
                // }
            }   
        }

        [Button]
        public void ResumeParticles()
        {
            if (_hasSystem && !_myParticleSystem.isEmitting)
            {
                if (_myParticleSystem.isStopped)
                {
                    _myParticleSystem.Play();
                }

                var em = _myParticleSystem.emission;
                em.enabled = true;
                // if (em.burstCount > 0)
                // {
                //     var count = em.GetBurst(0).count;
                //     _myParticleSystem.Emit((int) count.constant);
                // }
                _myParticleSystem.Emit(1);
                // _myParticleSystem.Clear(true);
                // _myParticleSystem.Stop(true, ParticleSystemStopBehavior.StopEmitting);
                // _myParticleSystem.Play(true);
            }
        }

        #endregion

        #region Private Methods

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
                p.remainingLifetime = KillImmediate ? Time.fixedDeltaTime : lifeAtExit;
                _exit[i] = p;
            }
            
            _myParticleSystem.SetTriggerParticles(ParticleSystemTriggerEventType.Exit, _exit);
        }

        #endregion
    }
}
