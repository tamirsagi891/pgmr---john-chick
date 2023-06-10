using BitStrap;
using Elad.Scripts;
using Elad.Scripts.Combat;
using UnityEngine;
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
        
        public virtual void OpenMenu()
        {
            var damageablePlayer = PlayerStatus.player.GetComponent<Damageable>();

            if (damageablePlayer.CheckPointsLives > 0)
            {
                damageablePlayer.CheckPointsLives -= 1;
                PlayerStatus.MenuManager.ReturnToLastCheckPoint();
            }

            else
            {
                menuUiObject.SetActive(true);
                EventSystem.current.SetSelectedGameObject(firstSelected); 
            }
            
        }

        public virtual void CloseMenu()
        {
            menuUiObject.SetActive(false);
        }

    }
}
