using System.Collections;
using UnityEngine;
using Logger = Nemesh.Logger;

namespace Mechanics.Enemies
{
    [AddComponentMenu("NPC/Effectors/Disable Detect For Time")]
    public class DisablePlayerDetectForTime : BaseNpcEffector
    {
        [SerializeField]
        private float timeToWait = 3f;

        protected override void ApplyEffect(BaseNpc npc)
        {
            StartCoroutine(DisableForTime(npc));
        }

        protected override void RemoveEffect(BaseNpc npc)
        {
            // This effect doesn't need to be removed
        }

        private IEnumerator DisableForTime(BaseNpc npc)
        {
            npc.CanDetectPlayer = false;
            Logger.Log($"{npc} Cant detect Player", Color.red);
            yield return new WaitForSeconds(timeToWait);
            npc.CanDetectPlayer = true;
            Logger.Log($"{npc} Can detect player", Color.blue);
        }
    }
}
