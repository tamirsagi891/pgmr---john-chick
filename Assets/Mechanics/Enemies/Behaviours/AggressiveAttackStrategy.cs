using UnityEngine;

namespace Mechanics.Enemies
{
    [AddComponentMenu("NPC/Attack Strategy/Aggressive")] // TODO: Created by ChatGPT
    public class AggressiveAttackStrategy : MonoBehaviour, IAttackStrategy
    {

        #region Inspector

        [SerializeField]
        private float attackDelay = 1.0f;

        [SerializeField]
        private float attackRange = 1.0f;

        [SerializeField]
        private float attackDamage = 10.0f;

        [SerializeField]
        private GameObject attackEffect;

        [SerializeField]
        private bool canAttackOnCooldown;

        #endregion

        #region Private Fields

        private BaseNpc _myNpc;
        private float _attackCooldown;
        private PlayerAttackController _target;

        #endregion

        #region IAttackStrategy

        public bool IsAttacking { get; private set; }

        public bool Attack()
        {
            if (_attackCooldown > 0.0f && !canAttackOnCooldown)
            {
                return false;
            }

            if (_target != null && Vector2.Distance(transform.position, _target.transform.position) <= attackRange)
            {
                IsAttacking = true;
                _attackCooldown = attackDelay;

                if (attackEffect != null)
                {
                    Instantiate(attackEffect, _target.transform.position, Quaternion.identity);
                }

                _target.GetComponent<StatsHandler>().TakeDamage(attackDamage);
                return true;
            }

            return false;
        }

        public void UpdateStrategy()
        {
            if (_target == null)
            {
                IsAttacking = false;
                return;
            }

            if (_attackCooldown > 0.0f)
            {
                _attackCooldown -= Time.deltaTime;
            }
            else
            {
                Attack();
            }
        }

        public void SetTarget(PlayerAttackController target)
        {
            _target = target;
        }

        #endregion

        #region MonoBehaviour

        private void Awake()
        {
            _myNpc = GetComponent<BaseNpc>();
        }

        private void Update()
        {
            UpdateStrategy();
        }

        #endregion

    }

}
