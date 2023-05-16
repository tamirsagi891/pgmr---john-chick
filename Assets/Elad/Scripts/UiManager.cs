using System;
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
            characterEvents.CharacterDamaged += CharacterTookDamage;
            characterEvents.CharacterHealed += CharacterHealed;

        }

        private void OnDisable()
        {
            characterEvents.CharacterDamaged -= CharacterTookDamage;
            characterEvents.CharacterHealed -= CharacterHealed;
        }

        public void CharacterTookDamage(GameObject character, int damageAmount)
        {
            InstantiateText(character, damageAmount);
        }
        
        public void CharacterHealed(GameObject character, int healthAmount)
        {
            InstantiateText(character, healthAmount);
        }

        private void InstantiateText(GameObject character, int amount)
        {
            Vector3 spawnPosition = Camera.main.WorldToScreenPoint(character.transform.position);
            TMP_Text tmpText = Instantiate(damageTextPrefab, spawnPosition, 
                quaternion.identity, _gameCanvas.transform).GetComponent<TMP_Text>();
            tmpText.text = amount.ToString();
        }
    }
}
