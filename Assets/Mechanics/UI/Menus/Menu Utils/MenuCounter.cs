using TMPro;
using UnityEngine;

namespace Mechanics.UI.Menus.Menu_Utils
{
    public class MenuCounter : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text count;


        public string Count
        {
            get => count.text;
            set => count.text = value;
        }
    }
}