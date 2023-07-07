using Mechanics.UI.FeatherUI;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Mechanics.UI.Menus
{
    [AddComponentMenu("Menus/Pause Menu")]
    public class PauseMenu : BaseMenuController
    {
        [Space]
        [SerializeField]
        private CollectiblePopup featherPopup;
        
        private void Awake()
        {
            EventSystem.current.SetSelectedGameObject(firstSelected);
        }

        public override void OpenMenu()
        {
            base.OpenMenu();
            if (featherPopup != null)
            {
                featherPopup.DoPopup(false);
            }
        }

        public override void CloseMenu()
        {
            base.CloseMenu();
            if (featherPopup != null)
            {
                featherPopup.ClearPopup();
            }
        }
    }
}
