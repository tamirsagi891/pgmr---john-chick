using UnityEngine;

namespace Mechanics.Enemies
{
    [AddComponentMenu("NPC/Base Npc", -1)]
    [SelectionBase]
    public class BaseNpc : MonoBehaviour
    {
        [Header("Base NPC Fields")]
        [ContextMenuItem("Log Stats", "TestContextMenu")]
        [SerializeField]
        [Tooltip("The data this NPC will reference")]
        protected NpcData myData;

        [SerializeField] 
        protected NpcAnimationControls animationControls;


        [ContextMenu("Log Stats")]
        public void TestContextMenu()
        {
            Debug.Log(myData, this);
        }

    }
}