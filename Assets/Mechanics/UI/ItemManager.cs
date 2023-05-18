using System;
using System.Collections.Generic;
using BitStrap;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Logger = Nemesh.Logger;

namespace Mechanics.UI
{
    [AddComponentMenu("UI/HUD Items")]
    public class ItemManager : MonoBehaviour
    {
        [SerializeField]
        private InputActionAsset uiInputs;
        
        [SerializeField]
        private List<GeneralItem> items = new() { new GeneralItem() };

        [SerializeField]
        [ReadOnly]
        private int currentItem;

        [Space]
        [SerializeField]
        [RequiredReference]
        private Image itemImage;

        public int CurrentItem
        {
            get => currentItem;
            set
            {
                currentItem = value;
                var item = items[currentItem];
                itemImage.sprite = item.sprite;
            }
        }

        private void OnEnable()
        {
            var map = uiInputs.FindActionMap("Items");
            var navigateAction = map.FindAction("NavigateItems");
            navigateAction.Enable();
            navigateAction.started += OnArrows;
            
            var useAction = map.FindAction("UseItem");
            useAction.Enable();
            useAction.started += OnUseItem;
        }

        private void OnDisable()
        {
            var map = uiInputs.FindActionMap("Items");
            var navigateAction = map.FindAction("NavigateItems");
            navigateAction.started -= OnArrows;
            navigateAction.Disable();
            
            var useAction = map.FindAction("UseItem");
            useAction.started -= OnUseItem;
            useAction.Disable();
        }

        public void OnArrows(bool left = false)
        {
            var value = left ? -1 : 1;
            CurrentItem = (items.Count + CurrentItem + value) % items.Count;
        }

        private void OnUseItem(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                var item = items[CurrentItem];
                item.UseItem();
            }
        }

        public void OnArrows(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                var value = (int) context.ReadValue<float>();
                CurrentItem = (items.Count + CurrentItem + value) % items.Count;
            }
        }
    }
}