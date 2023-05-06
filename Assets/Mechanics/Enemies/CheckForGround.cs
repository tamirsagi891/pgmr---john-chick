using System;
using UnityEngine;
using Logger = Nemesh.Logger;

namespace Mechanics.Enemies
{
    [AddComponentMenu("NPC/Checkers/Ground")]
    public class CheckForGround : MonoBehaviour
    {

        #region Inspector

        [SerializeField]
        private BaseNpc npcToReportTo;

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
            npcToReportTo.GroundContacts += 1;
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            npcToReportTo.GroundContacts -= 1;
        }

        #endregion

        #region Private Methods

        private void Disable()
        {
            gameObject.SetActive(false);
        }

        #endregion
    }
}
