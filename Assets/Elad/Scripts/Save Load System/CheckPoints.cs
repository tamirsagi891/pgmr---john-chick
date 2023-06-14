using UnityEngine;
using UnityEngine.InputSystem;
using Logger = Nemesh.Logger;

namespace Elad.Scripts.Save_Load_System
{
    public class CheckPoints : MonoBehaviour
    {
        private Vector3 _position;

        public Vector3 Position
        {
            get => _position;
            set => _position = value;
        }


        private void Awake()
        {
            Position = transform.position;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag(TagStrings.playerTag))
            {
                PlayerStatus.PlayerInsideCheckPoint = true;
                PlayerStatus.LastCheckPoint = this;
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag(TagStrings.playerTag))
            {
                PlayerStatus.PlayerInsideCheckPoint = false;
            }
        }
    }
}