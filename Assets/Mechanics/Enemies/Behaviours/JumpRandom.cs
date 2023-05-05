using System;
using System.Collections;
using BitStrap;
using UnityEngine;
using Logger = Nemesh.Logger;
using Random = UnityEngine.Random;

namespace Mechanics.Enemies
{
    
    [AddComponentMenu("NPC/Behaviours/Jump Random")]  // TODO: add for random dash, death, stop/start movement, etc
    public class JumpRandom : MonoBehaviour
    {

        #region Inspector

        [SerializeField]
        [Min(0)]
        private float factor = 3f;
        
        [Space]
        [SerializeField]
        private BaseNpc npcToReportTo;

        #endregion
        

        #region MonoBehaviour

        private void Start()
        {
            StartCoroutine(Jump(factor * Random.value + 0.5f));
        }

        #endregion

        #region Courotines

        private IEnumerator Jump(float timer)
        {
            while (true)
            {
                yield return new WaitForSeconds(timer);
                npcToReportTo.Jump();
            }
        }

        #endregion

    }
}
