using System;
using BitStrap;
using Elad.Scripts.Events;
using UnityEngine;
using Logger = Nemesh.Logger;

namespace Elad.Scripts
{
    public class PlayerJoice : MonoBehaviour
    {
        [SerializeField] private ParticleSystem dustParticleSystem;
        [SerializeField] private ParticleSystem jumpParticleSystem;

        private void OnEnable()
        {
            ParticleEvents.PlayerDust.AddListener(PlayChangeDirectionParticleS);
            ParticleEvents.PlayerJump.AddListener(PlayJumpParticleS);
        }

        private void OnDisable()
        {
            ParticleEvents.PlayerDust.RemoveListener(PlayChangeDirectionParticleS);
            ParticleEvents.PlayerJump.RemoveListener(PlayJumpParticleS);
        }
        


        [Button]
        private void PlayChangeDirectionParticleS()
        {
            dustParticleSystem.Play();
        }
        
        private void PlayJumpParticleS()
        {
            jumpParticleSystem.Play();
        }
    }
}
