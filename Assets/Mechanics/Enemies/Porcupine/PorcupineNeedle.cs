using System;
using BitStrap;
using UnityEngine;

namespace Mechanics.Enemies.Porcupine
{
    [AddComponentMenu("NPC/Attack Controls/Porcupine Needle")]
    [RequireComponent(typeof(Rigidbody2D))]
    public class PorcupineNeedle : Projectile
    {
        [Header("Porcupine Needle")]
        [SerializeField]
        [TagSelector]
        private string playerTag = "Player";

        protected Rigidbody2D MyRigidbody;
        protected SpriteRenderer MyRenderer;
        protected bool HasSpriteRenderer;


        protected virtual void Awake()
        {
            MyRigidbody = GetComponent<Rigidbody2D>();
            HasSpriteRenderer = TryGetComponent(out MyRenderer);
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (!other.gameObject.CompareTag(playerTag))
            {
                return;
            }
            
            if (other.gameObject.TryGetComponent(out ICanBeAttacked target))
            {
                Attack(target);
            }
        }

        #region Public Methods

        public override void Shot(Vector3 position)
        {
            base.Shot(position);
            var direction = Parameters.Direction switch
            {
                Direction.Left => Vector2.left,
                Direction.Right => Vector2.right,
                _ => Vector2.right
            };
            MyRigidbody.velocity = direction * Parameters.ShotSpeed;
        }

        protected override void SetDirection()
        {
            if (HasSpriteRenderer)
            {
                MyRenderer.flipX = Parameters.Direction == Direction.Left;
                return;
            }

            base.SetDirection();
        }

        #endregion

    }
}
