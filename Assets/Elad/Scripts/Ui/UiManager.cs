using System;
using Elad.Events;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace Elad.Scripts
{
    public class UiManager : MonoBehaviour
    {
        [SerializeField]
        private InputActionAsset uiInputs;
        [SerializeField] private GameObject damageTextPrefab;
        [SerializeField] private GameObject healthTextPrefab;
        
        [SerializeField]
        private Canvas numbersCanvas;

        [Header("Events")]
        [SerializeField]
        private UnityEvent<InputAction.CallbackContext> onPauseEvent;


        #region MonoBehaviour

        private void Awake()
        {
            if (numbersCanvas == null)
            {
                numbersCanvas = FindObjectOfType<Canvas>();
            }
        }

        private void OnEnable()
        {
            characterEvents.CharacterDamaged.AddListener(CharacterTookDamage);
            characterEvents.CharacterHealed.AddListener(CharacterHealed);
            
            var map = uiInputs.FindActionMap("UI");
            var pauseAction = map.FindAction("Pause");
            pauseAction.Enable();
            pauseAction.started += OnPause;
        }

        private void OnDisable()
        {   
            characterEvents.CharacterDamaged.RemoveListener(CharacterTookDamage);
            characterEvents.CharacterHealed.RemoveListener(CharacterHealed);
            
            var map = uiInputs.FindActionMap("UI");
            var pauseAction = map.FindAction("Pause");
            pauseAction.started -= OnPause;
            pauseAction.Disable();
            
        }

        #endregion

        #region Input Callbacks

        private void OnPause(InputAction.CallbackContext context)
        {
            onPauseEvent.Invoke(context);
        }

        #endregion

        #region Public Methods

        public void CharacterTookDamage(GameObject character, int damageAmount)
        {
            InstantiateText(character, damageAmount, false);
        }
        
        public void CharacterHealed(GameObject character, int healthAmount)
        {
            InstantiateText(character, healthAmount, true);
        }

        #endregion

        #region Private Methods

        private void InstantiateText(GameObject character, int amount, bool heal)
        {
            Vector3 spawnPosition = Camera.main.WorldToScreenPoint(character.transform.position);
            GameObject textPrefab;
            switch (heal)
            {
                     
                case true:
                    textPrefab = healthTextPrefab;
                    break;
                
                case false:
                    textPrefab = damageTextPrefab;
                    break;
            }
            TMP_Text tmpText = Instantiate(textPrefab, spawnPosition, 
                quaternion.identity, numbersCanvas.transform).GetComponent<TMP_Text>();
            tmpText.text = amount.ToString();
        }

        #endregion
    }
}
