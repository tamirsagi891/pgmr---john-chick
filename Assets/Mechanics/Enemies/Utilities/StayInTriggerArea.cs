using System;
using System.Collections.Generic;
using System.Linq;
using BitStrap;
using UnityEngine;
using UnityEngine.Serialization;
using Logger = Nemesh.Logger;

namespace Mechanics.Enemies
{
    [AddComponentMenu("NPC/Checkers/Stay In Area")]
    [RequireComponent(typeof(Collider2D))]
    public class StayInTriggerArea : MonoBehaviour
    {
        #region Inspector

        [FormerlySerializedAs("npcToReportTo")]
        [Space]
        [SerializeField]
        private List<BaseNpc> npcToReportToList;

        #endregion

        #region Private Fields

        private HashSet<BaseNpc> _outside = new();

        #endregion

        #region MonoBehaviour

        private void OnEnable()
        {
            foreach (var npc in npcToReportToList)
            {
                npc.events.onDisable.AddListener(Disable);
            }
        }

        private void OnDisable()
        {
            foreach (var npc in npcToReportToList)
            {
                npc.events.onDisable.RemoveListener(Disable);
            }
        }

        private void FixedUpdate()
        {
            foreach (var npc in _outside)
            {
                npc.PlayerContact = null;
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            foreach (var npc in npcToReportToList.Where(npc => npc.gameObject == other.gameObject))
            {
                Logger.Log("There");
                _outside.Remove(npc);
                break;
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            Logger.Log(other.gameObject == npcToReportToList[0].gameObject);
            foreach (var npc in npcToReportToList.Where(npc => npc.gameObject == other.gameObject))
            {
                Logger.Log("here");
                _outside.Add(npc);
                break;
            }
        }

        #endregion

        #region Private Methods

        private void Disable(BaseNpc npc)
        {
            npcToReportToList.Remove(npc);
            _outside.Remove(npc);
            if (npcToReportToList.Count == 0)
            {
                gameObject.SetActive(false);
            }
        }

        #endregion
    }
}