using BitStrap;
using UnityEngine;

namespace Mechanics.Enemies
{
    [AddComponentMenu("NPC/Checkers/Player")]
    [RequireComponent(typeof(Collider2D))]
    public class CheckForPlayer : MonoBehaviour
    {

        #region Inspector

        [HelpBox(@"This doesnt have to be attached to the NPC!
You can put it as separate GameObject and connect to the NPC that way.
InThat case, we might want to change to list and stop the disable",
            HelpBoxAttribute.MessageType.Info)]
        [Space]
        [SerializeField]
        private BaseNpc npcToReportTo;

        #endregion

        #region Private Fields

        private Collider2D _myCollider;

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

        private void Start() // TODO: make this also the attack strategy controller? or separate object?
        {
            _myCollider = GetComponent<Collider2D>();
            var col = _myCollider as CircleCollider2D;
            if (col != null)
            {
                col.radius = npcToReportTo.NpcDataScriptable.stats.detectionRadius;
            }
            else
            {
                var colBox = _myCollider as BoxCollider2D;
                if (colBox == null)
                {
                    return;
                }
                colBox.size = new Vector2(npcToReportTo.NpcDataScriptable.stats.detectionRadius, colBox.size.y);
                colBox.offset = new Vector2(npcToReportTo.NpcDataScriptable.stats.detectionRadius / 2f, colBox.offset.y);
            }
            
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            var playerController = other.GetComponent<ICanBeAttacked>(); // TODO: move to using Tags instead.
            if (playerController != null)
            {
                npcToReportTo.PlayerContact = playerController;
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            var playerController = other.GetComponent<ICanBeAttacked>();
            if (playerController != null && npcToReportTo.PlayerContact == playerController)
            {
                npcToReportTo.PlayerContact = null; // TODO: AddFeatherToPlayer to attack targets instead?
            }
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
