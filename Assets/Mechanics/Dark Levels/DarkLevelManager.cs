using System;
using System.Collections.Generic;
using BitStrap;
using Managers;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Mechanics.Dark_Levels
{
    public class DarkLevelManager : MonoBehaviour
    {
        [SerializeField]
        private InputAction secretDarkSwitch;
        
        [SerializeField]
        private List<GameObject> objectToEnable;

        [SerializeField]
        private List<GameObject> objectToDisable;

        public static bool isCurrentLevelDark;
        
        private void Awake()
        {
            if (!GeneralGameManager.LoadAsDark)
            {
                isCurrentLevelDark = false;
                return;
            }

            SetDarkHelper();
        }

        private void OnEnable()
        {
            secretDarkSwitch.Enable();
            secretDarkSwitch.started += SwitchDarkness;
        }

        private void OnDisable()
        {
            secretDarkSwitch.started -= SwitchDarkness;
            secretDarkSwitch.Disable();
        }

        private void SwitchDarkness(InputAction.CallbackContext obj)
        {
            if (isCurrentLevelDark)
            {
                UnsetDarkHelper();
            }
            else
            {
                SetDarkHelper();
            }
        }


        [Button("Set Dark")]
        public void SetDarkHelper()
        {
            isCurrentLevelDark = true;
            GeneralGameManager.LoadAsDark = false;
            foreach (var obj in objectToDisable)
            {
                obj.SetActive(false);
            }

            foreach (var obj in objectToEnable)
            {
                obj.SetActive(true);
            }
        }
        
        [Button("Unset Dark")]
        public void UnsetDarkHelper()
        {
            isCurrentLevelDark = false;
            GeneralGameManager.LoadAsDark = false;
            foreach (var obj in objectToDisable)
            {
                obj.SetActive(true);
            }

            foreach (var obj in objectToEnable)
            {
                obj.SetActive(false);
            }
        }
        
    }
}