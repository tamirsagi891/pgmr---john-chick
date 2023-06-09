using UnityEngine;
using UnityEngine.EventSystems;

namespace Mechanics.UI.Menus
{
    [AddComponentMenu("Menus/Pause Menu")]
    public class PauseMenu : BaseMenuController
    {
        private void Awake()
        {
            EventSystem.current.SetSelectedGameObject(firstSelected);
        }
    }
}
