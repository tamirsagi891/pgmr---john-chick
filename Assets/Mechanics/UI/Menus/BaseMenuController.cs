using BitStrap;
using UnityEngine;
using UnityEngine.EventSystems;

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
