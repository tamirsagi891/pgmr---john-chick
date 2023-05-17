using System;
using BitStrap;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Mechanics.UI
{
    [AddComponentMenu("UI/HUD Health")]
    [SelectionBase]
    public class HealthHudManager : MonoBehaviour
    {
        [Space]
        [Header("References")]
        [SerializeField]
        [RequiredReference]
        private TMP_Text healthText;

        [SerializeField]
        [RequiredReference]
        private Image healthFront;

        // [SerializeField]
        // [RequiredReference]
        // private Image healthBackground;

        // [SerializeField]
        // private bool useFill = true;

        [Header("Health Controls")]
        [SerializeField]
        [Range(0, 1)]
        private float fillAmount = 1f;

        public int MaxHealth { get; set; } = 100;

        public float FillAmount
        {
            get => fillAmount;
            set
            {
                fillAmount = value;
                healthFront.fillAmount = value;
                // if (!useFill)
                // {
                //     healthFront.rectTransform = (1 - value) * _backgroundW;
                // }
                healthText.text = $"{Mathf.CeilToInt(MaxHealth * FillAmount):N0}";
            }
        }

        // public bool UseFill
        // {
        //     get => useFill;
        //     set
        //     {
        //         useFill = value;
        //         healthFront.type = useFill ? Image.Type.Filled : Image.Type.Sliced;
        //     }
        // }

        private float _backgroundW;

        private void OnValidate()
        {
            // _backgroundW = healthBackground.rectTransform.rect.width;
            // UseFill = useFill;
            FillAmount = fillAmount;
        }

        private void Awake()
        {
            // _backgroundW = healthBackground.rectTransform.rect.width;
        }
    }
}