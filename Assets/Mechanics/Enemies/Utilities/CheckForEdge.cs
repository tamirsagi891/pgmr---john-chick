using UnityEngine;

namespace Mechanics.Enemies
{
    [AddComponentMenu("NPC/Checkers/Edge")]
    public class CheckForEdge : MonoBehaviour
    {

        #region Inspector

        [SerializeField]
        private BaseNpc npcToReportTo;

        #endregion

        #region Private Method

        private void Disable()
        {
            gameObject.SetActive(false);
        }

        #endregion

        #region Private Fields

        private int _groundCounter; // TODO: list?

        private int GroundCounter
        {
            get => _groundCounter;
            set
            {
                _groundCounter = value;
                npcToReportTo.EdgeInFront = _groundCounter == 0;
            }
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
            GroundCounter += 1;
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            GroundCounter -= 1;
        }

        #endregion

    }
}
