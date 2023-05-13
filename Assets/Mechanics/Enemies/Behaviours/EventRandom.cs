using System.Collections;
using Avrahamy.EditorGadgets;
using UnityEngine;
using UnityEngine.Events;

namespace Mechanics.Enemies
{
    [AddComponentMenu("NPC/Behaviours/Event Random")]
    public class EventRandom : MonoBehaviour
    {
        #region Inspector

        [SerializeField]
        [Min(0)]
        private float factor = 3f;

        [SerializeField]
        private bool jump;
        
        [SerializeField]
        private bool dash;
        
        [SerializeField]
        private bool attack;
        
        [SerializeField]
        private bool switchDirection;
        
        [SerializeField]
        private bool stopMove;

        [SerializeField]
        [ConditionalHide("stopMove")]
        private float stopTime = 1f;

        [SerializeField]
        private UnityEvent<BaseNpc> onRandomEvent;

        [Space]
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

        private void Start()
        {
            StartCoroutine(EventOnRandomTime());
        }

        #endregion

        #region Private Methods

        private void Disable()
        {
            gameObject.SetActive(false);
        }

        #endregion

        #region Courotines

        private IEnumerator EventOnRandomTime()
        {
            while (true)
            {
                var timer = factor * Random.value + 0.5f;
                yield return new WaitForSeconds(timer);
                if (jump)
                {
                    npcToReportTo.Jump();
                }

                if (dash)
                {
                    npcToReportTo.Dash();
                }

                if (attack)
                {
                    npcToReportTo.Attack();
                }

                if (switchDirection)
                {
                    npcToReportTo.HandleDirectionSwitch();
                }

                if (stopMove)
                {
                    npcToReportTo.SwitchMovement(stopTime);
                }
                onRandomEvent.Invoke(npcToReportTo);
            }
        }

        #endregion
    }
}