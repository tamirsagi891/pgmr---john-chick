using System;
using BitStrap;
using Elad.Scripts.Events;
using UnityEngine;
using Logger = Nemesh.Logger;

namespace Elad.Scripts
{
    public class PlayerJoice : MonoBehaviour
    {
        [SerializeField] private float minVelocityForDustParticles = 0.5f;
        [SerializeField] private float maxVelocityDustParticle;
        [SerializeField] private ParticleSystem dustParticleSystem;
        [SerializeField] private ParticleSystem jumpParticleSystem;

        [SerializeField] private ParticleSystem glideBackParticleSystem;
        [SerializeField] private ParticleSystem glideFrontParticleSystem;
        private Rigidbody2D _playerRigidBody;

        private void OnEnable()
        {
            ParticleEvents.PlayerChangeDirection.AddListener(PlayChangeDirectionParticleS);
            ParticleEvents.PlayerJump.AddListener(PlayJumpParticleS);
            ParticleEvents.PlayerGlide.AddListener(PlayGlideParticles);
        }

        private void OnDisable()
        {
            ParticleEvents.PlayerChangeDirection.RemoveListener(PlayChangeDirectionParticleS);
            ParticleEvents.PlayerJump.RemoveListener(PlayJumpParticleS);
            ParticleEvents.PlayerGlide.RemoveListener(PlayGlideParticles);
        }

        private void Start()
        {
            _playerRigidBody = PlayerStatus.Player.GetComponent<Rigidbody2D>();
        }


        private void PlayChangeDirectionParticleS()
        {
            dustParticleSystem.Clear();
            if (!PlayerStatus.IsGrounded) return;
            // Get the Velocity over lifetime modult
            ParticleSystem.VelocityOverLifetimeModule snowVelocity = dustParticleSystem.velocityOverLifetime;

            //And to modify the value
            ParticleSystem.MinMaxCurve rate = new ParticleSystem.MinMaxCurve();


            var playerVelocityX = Math.Abs(_playerRigidBody.velocity.x);

            if (playerVelocityX < minVelocityForDustParticles) return;
            playerVelocityX = Math.Min(playerVelocityX, maxVelocityDustParticle);

            rate.constantMax = playerVelocityX;
            // rate.constantMax = Math.Max(1, playerVelocityX); // or whatever value you want
            rate.constantMax *= PlayerStatus.isFacingRight ? 1 : -1;
            snowVelocity.x = rate;

            dustParticleSystem.Play();
        }

        private void PlayJumpParticleS()
        {
            jumpParticleSystem.Play();
        }

        private void PlayGlideParticles(bool mode)
        {
            if (mode)
            {
                glideBackParticleSystem.Play();
                glideFrontParticleSystem.Play();
            }

            else
            {
                // glideBackParticleSystem.Clear();
                glideBackParticleSystem.Stop();
                // glideFrontParticleSystem.Clear();
                glideFrontParticleSystem.Stop();
            }
        }
    }
}