using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Mechanics.UI
{
    public class SettingsMenu : MonoBehaviour
    {
        [SerializeField]
        public SoundEvents soundEvents;

        public void OnMasterChange(float value)
        {
            soundEvents.onMasterChange.Invoke(value);
        }
        
        public void OnMusicChange(float value)
        {
            soundEvents.onMusicChange.Invoke(value);
        }
    }

    [Serializable]
    public class SoundEvents
    {
        public UnityEvent<float> onMasterChange = new();
        public UnityEvent<float> onMusicChange = new();
    }
}