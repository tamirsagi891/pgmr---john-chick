using UnityEngine;

namespace Mechanics.Tutorial_Keys
{
    [CreateAssetMenu(fileName = "ControllerSprites", menuName = "Tutorial/Sprites", order = 0)]
    public class ControllerSprites : ScriptableObject
    {
        [SerializeField]
        public string jump;

        [SerializeField]
        public string left;

        [SerializeField]
        public string right;

        [SerializeField]
        public string crouch;

        [SerializeField]
        public string save;

        [SerializeField]
        public string back;
    }
}