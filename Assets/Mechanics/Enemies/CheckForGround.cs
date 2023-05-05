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

        private void OnTriggerEnter2D(Collider2D other)
        {
            npcToReportTo.GroundContacts += 1;
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            npcToReportTo.GroundContacts -= 1;
        }

        #endregion
    }
}
