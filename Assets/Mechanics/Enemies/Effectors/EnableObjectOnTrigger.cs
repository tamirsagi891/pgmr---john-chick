using UnityEngine;

namespace Mechanics.Enemies
{
    [AddComponentMenu("NPC/Effectors/Enable Object On Trigger")]
    public class EnableObjectOnTrigger: BaseNpcEffector
    {
        [SerializeField]
        private bool setState = true;
        
        [SerializeField]
        private bool disableOnExit;

        [SerializeField]
        private GameObject otherObject;

        protected override void ApplyEffect(BaseNpc npc)
        {
            otherObject.SetActive(setState);
        }

        protected override void RemoveEffect(BaseNpc npc)
        {
            if (disableOnExit)
            {
                otherObject.SetActive(!setState);
            }
        }
    }
}