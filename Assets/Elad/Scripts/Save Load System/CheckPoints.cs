using Elad.Events;
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


        private void OnEnable()
        {
            characterEvents.FunctionsSave.AddListener(OpenOrCloseCheckPoint);
        }

        private void OnDisable()
        {
            characterEvents.FunctionsSave.RemoveListener(OpenOrCloseCheckPoint);
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

        private void OpenOrCloseCheckPoint()
        {
            Logger.Log("kaka");
            if (PlayerStatus.LastCheckPoint == this)
            {
                
            }

            else
            {
                
            }
        }
    }
}