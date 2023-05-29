using System;
using Elad.Events;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;

namespace Elad.Scripts
{
    public class UiManager : MonoBehaviour
    {
        [SerializeField] private GameObject damageTextPrefab;
        [SerializeField] private GameObject healthTextPrefab;

        private Canvas _gameCanvas;


        private void Awake()
        {
            _gameCanvas = FindObjectOfType<Canvas>();
            
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

        public void CharacterTookDamage(GameObject character, int damageAmount)
        {
            InstantiateText(character, damageAmount, false);
        }
        
        public void CharacterHealed(GameObject character, int healthAmount)
        {
            InstantiateText(character, healthAmount, true);
        }

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
                quaternion.identity, _gameCanvas.transform).GetComponent<TMP_Text>();
            tmpText.text = amount.ToString();
        }
    }
}
