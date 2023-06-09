using Elad.Events;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Mechanics.UI.Menus
{
    [AddComponentMenu("Menus/UI Manager™️ (by Elad)")]
    public class UiManager : MonoBehaviour
    {
        [SerializeField] private GameObject damageTextPrefab;
        [SerializeField] private GameObject healthTextPrefab;

        [SerializeField]
        private bool createDamageText;
        
        [SerializeField]
        private Canvas numbersCanvas;

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
        }

        private void OnDisable()
        {   
            characterEvents.CharacterDamaged.RemoveListener(CharacterTookDamage);
            characterEvents.CharacterHealed.RemoveListener(CharacterHealed);
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
            if (!createDamageText)
            {
                return;
            }
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
