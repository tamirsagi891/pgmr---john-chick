using UnityEngine;

namespace Mechanics.Enemies
{

    [RequireComponent(typeof(Collider2D))]
    public abstract class BaseNpcEffector : MonoBehaviour
    {

        #region Inspector

        [Header("Base Npc Effector")]
        [SerializeField]
        private bool effectActive = true;

        #endregion

        #region MonoBehaviour

        protected virtual void OnTriggerEnter2D(Collider2D other)
        {
            if (!effectActive)
            {
                return;
            }

            var npc = other.GetComponentInParent<BaseNpc>();
            if (npc == null)
            {
                return;
            }

            ApplyEffect(npc);
        }

        protected virtual void OnTriggerExit2D(Collider2D other)
        {
            if (!effectActive)
            {
                return;
            }

            var npc = other.GetComponentInParent<BaseNpc>();
            if (npc == null)
            {
                return;
            }

            RemoveEffect(npc);
        }

        #endregion

        #region Effector Interface

        protected abstract void ApplyEffect(BaseNpc npc);

        protected abstract void RemoveEffect(BaseNpc npc);

        #endregion

    }
}
