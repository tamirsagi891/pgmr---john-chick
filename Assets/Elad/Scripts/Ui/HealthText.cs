using System;
using TMPro;
using UnityEngine;

namespace Elad.Scripts
{
    public class HealthText : MonoBehaviour
    {
        [Header("Component's")]
        private RectTransform _rectTransform;

        private TextMeshProUGUI _textMeshProUGUI;
        private Color _originalColor;

        
        [Header("Speed")]
        [SerializeField] Vector3 moveSpeed = Vector3.up;

        [Header("Times")] [SerializeField] private float fadeTime = 1f;
        private float _fadeTimer;
        
        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            _textMeshProUGUI = GetComponent<TextMeshProUGUI>();
            _originalColor = _textMeshProUGUI.color;
        }
        

        // Update is called once per frame
        void Update()
        {
            //Movement
            _rectTransform.position += moveSpeed * Time.deltaTime;
            
            //Fade
            _fadeTimer += Time.deltaTime;
            if (_fadeTimer < fadeTime)
            {
                float newAlpha = _originalColor.a * (1- (_fadeTimer / fadeTime));
                _textMeshProUGUI.color = new Color(_originalColor.r, _originalColor.g, _originalColor.b, newAlpha);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}
