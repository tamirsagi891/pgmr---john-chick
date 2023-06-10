using System;
using Avrahamy.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using Logger = Nemesh.Logger;

namespace Mechanics.Tutorial_Keys
{
    public class TutorialKeyRegister : MonoBehaviour
    {
        #region Inspector

        [SerializeField]
        private InputActionAsset uiInputs;  // TODO: put the inputs
        
        [SerializeField]
        private Color pressedColor = Color.yellow;

        #endregion

        #region Private Fields

        private TMP_Text[] _myTexts;

        #endregion

        #region Static Fields

        private const string SpriteFormat = "<sprite tint=1 name=%>";

        private static string GetSpriteFormatWithName(string spriteName)
        {
            return SpriteFormat.Replace("%", spriteName);
        }
        
        private static string GetSpriteFormatWithNameAndColor(string spriteName, Color color)
        {
            var format = GetSpriteFormatWithName(spriteName);
            return GetFormatWithColor(format, color);
        }
        
        private static string GetFormatWithColor(string format, Color color)
        {
            return $"<color=#{color.ToHex()}>{format}</color>";
        }

        #endregion

        #region MonoBehaviour

        private void Awake()
        {
            _myTexts = GetComponentsInChildren<TMP_Text>();
        }

        private void OnEnable()
        {
            var map = uiInputs.FindActionMap("Player");
            var moveAction = map.FindAction("Move");
            moveAction.started += OnMove;
            moveAction.canceled += OnMove;
            
            var crouchAction = map.FindAction("Crouch");
            crouchAction.started += OnCrouch;
            crouchAction.canceled += OnCrouch;
            
            var jumpAction = map.FindAction("Jump");
            jumpAction.started += OnJump;
            jumpAction.canceled += OnJump;
        }

        private void OnDisable()
        {
            
            var map = uiInputs.FindActionMap("Player");
            var moveAction = map.FindAction("Move");
            moveAction.started -= OnMove;
            moveAction.canceled -= OnMove;
            
            var crouchAction = map.FindAction("Crouch");
            crouchAction.started -= OnCrouch;
            crouchAction.canceled -= OnCrouch;
            
            var jumpAction = map.FindAction("Jump");
            jumpAction.started -= OnJump;
            jumpAction.canceled -= OnJump;
        }

        #endregion

        #region Input Callbacks

        private void OnMove(InputAction.CallbackContext context)
        {
            var direction = context.ReadValue<Vector2>();
            MarkDirections(direction);
        }
        
        private void OnCrouch(InputAction.CallbackContext context)
        {
            MarkSprite(context.started, context.control.name);
        }
        
        private void OnJump(InputAction.CallbackContext context)
        {
            MarkSprite(context.started, context.control.name);
        }

        #endregion
        
        #region Markers

        private void MarkDirections(Vector2 direction)
        {
            MarkSprite(direction.x > 0, "rightArrow");
            MarkSprite(direction.x < 0, "leftArrow");
        }

        private void MarkSprite(bool state, string spriteName)
        {
            var format = GetSpriteFormatWithName(spriteName);
            var coloredFormat = GetFormatWithColor(format, pressedColor);
            if (state)
            {
                foreach (var tmpText in _myTexts)
                {
                    tmpText.text = tmpText.text.Replace(format, coloredFormat);
                }
                return;
            }
            foreach (var tmpText in _myTexts)
            {
                tmpText.text = tmpText.text.Replace(coloredFormat, format);
            }
        }
        
        #endregion
    }
}