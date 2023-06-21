using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Elad.Scripts.Lights
{
    public class LightsManager : MonoBehaviour
    {
        [SerializeField] private Light2D globalLight;
        [SerializeField] private Light2D redLight;
        [SerializeField] private Light2D playerEnvironmentLight;


        private void Awake()
        {
            LightsStatus.LightsManager = this;
            LightsStatus.GlobalLight = globalLight;
            LightsStatus.RedLightPlayer = redLight;
            LightsStatus.EnvironmentLightPlayer = playerEnvironmentLight;
        }
        
        
    }
}
