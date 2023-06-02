using System;
using Avrahamy;
using BitStrap;
using Mechanics.Enemies.Porcupine;
using UnityEngine;
using Logger = Nemesh.Logger;

namespace Mechanics.Enemies
{
    [AddComponentMenu("NPC/Porcupine")]
    public class PorcupineEnemy : BaseNpc
    {
        [Header("Porcupine")]
        [SerializeField]
        [RequiredReference]
        private NeedleShooter needleController;
        
        [SerializeField]
        private int burstCount = 1;

        [SerializeField]
        private float timeBetweenBursts = 0.25f;
        
        private void Start()
        {
            var timeAttacker = attackController as AttackOnTimer;
            if (timeAttacker != null)  // TODO: pass Config struct to all AttackBehaviours instead
            {
                timeAttacker.Cooldown = MyStatsHandler.CurrentStats.cooldown;
            }
        }

        protected override void AttackAllTarget()
        {
            events.onAttack.Invoke();
            var attackParameters = GetAttackParameters();
            // TODO: animationControls.Attack = true; for each burst
            needleController.BurstAttack(burstCount, timeBetweenBursts, attackParameters);
        }
    }
}
