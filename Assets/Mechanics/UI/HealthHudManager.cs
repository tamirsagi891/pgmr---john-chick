using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Mechanics.UI
{
    public class HealthHudManager : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text healthText;

        [SerializeField]
        private GameObject healthContainer;

        [SerializeField]
        private GameObject healthRepresentObject;

        [SerializeField]
        public UnityEvent<float> onHealthChange;
    }
}