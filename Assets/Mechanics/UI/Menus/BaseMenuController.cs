using System;
using BitStrap;
using Elad.Scripts;
using Elad.Scripts.Combat;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using Logger = Nemesh.Logger;

namespace Mechanics.UI.Menus
{
    [AddComponentMenu("Menus/Base Menu Controller", -1)]
    public class BaseMenuController : MonoBehaviour
    {
        [Header("Menu Controller")]
        [SerializeField]
        [RequiredReference]
        public GameObject menuUiObject;

        [SerializeField]
        [RequiredReference]
        protected GameObject firstSelected;

        [Space]
        [SerializeField]
        private MenuEvents events;
        
        public virtual void OpenMenu()
        {
            menuUiObject.SetActive(true);
            EventSystem.current.SetSelectedGameObject(firstSelected);
            events.onOpen.Invoke();
        }

        public virtual void CloseMenu()
        {
            menuUiObject.SetActive(false);
            events.onClose.Invoke();
        }

    }

    [Serializable]
    public struct MenuEvents
    {
        [SerializeField]
        public UnityEvent onOpen;

        [SerializeField]
        public UnityEvent onClose;
    }
}
