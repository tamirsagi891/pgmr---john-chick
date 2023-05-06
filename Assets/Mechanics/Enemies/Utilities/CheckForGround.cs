using UnityEngine;

namespace Mechanics.Enemies
{
    [AddComponentMenu("NPC/Checkers/Ground")]
    public class CheckForGround : MonoBehaviour
    {

        #region Inspector

        [SerializeField]
        private BaseNpc npcToReportTo;

        #endregion

        #region Private Fields

        private int _groundContacts;

        #endregion

        #region Private Methods

        private void Disable()
        {
            gameObject.SetActive(false);
        }

        #endregion

        #region MonoBehaviour

        private void OnEnable()
        {
            npcToReportTo.events.onDeath.AddListener(Disable);
        }

        private void OnDisable()
        {
            npcToReportTo.events.onDeath.RemoveListener(Disable);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            _groundContacts++;
            npcToReportTo.IsGrounded = true;
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            _groundContacts--;
            npcToReportTo.IsGrounded = _groundContacts > 0;
        }

        #endregion

    }
}
