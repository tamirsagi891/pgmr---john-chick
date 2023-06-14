using UnityEngine;
using UnityEngine.InputSystem;

namespace Elad.Scripts.Save_Load_System
{
    public class CheckPoints : MonoBehaviour
    {
        [SerializeField]
        private InputActionAsset uiInputs;
    
        private Vector3 _position;

        public Vector3 Position
        {
            get => _position;
            set => _position = value;
        }

        private void OnEnable()
        {
            var map = uiInputs.FindActionMap("Player");
            var moveAction = map.FindAction("Move");
            moveAction.started += OnMove;
            moveAction.canceled += OnMove;
            
        }

        private void OnDisable()
        {
            
            var map = uiInputs.FindActionMap("Player");
            var moveAction = map.FindAction("Move");
            moveAction.started -= TryToSave;
            moveAction.canceled -= TryToSave;
            
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

        private void TryToSave()
        {
            
        }
    
    }
}