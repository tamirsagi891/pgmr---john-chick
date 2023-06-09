using BitStrap;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

namespace Mechanics.UI
{
    public class BaseMenuController : MonoBehaviour
    {
        [Header("Menu Controller")]
        [SerializeField]
        [RequiredReference]
        public GameObject menuUiObject;

        [SerializeField]
        [RequiredReference]
        protected GameObject firstSelected;
        
        public virtual void OpenMenu()
        {
            menuUiObject.SetActive(true);
            EventSystem.current.SetSelectedGameObject(firstSelected);
        }

        public virtual void CloseMenu()
        {
            menuUiObject.SetActive(false);
        }

    }
}
