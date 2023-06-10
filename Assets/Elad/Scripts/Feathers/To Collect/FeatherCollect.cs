using Elad.Events;
using UnityEngine;

namespace Elad.Scripts.Arrows
{
    [RequireComponent(typeof(Collectable))]
    public class FeatherToCollect : MonoBehaviour
    {
        [SerializeField] private FeathersManager.FeatherKind myFeatherKind;
        
        
        private Vector3 _position;

        public Vector3 Position
        {
            get => _position;
            set => _position = value;
        }

        public FeathersManager.FeatherKind MyFeatherKind
        {
            get => myFeatherKind;
            set => myFeatherKind = value;
        }


     

        private void Start()
        {
            if (PlayerStatus.InitializeFromJason)
            {
                // Destroy(gameObject);
            }

            if (PlayerStatus.FeathersToCollectManager)
            {
                PlayerStatus.FeathersToCollectManager.AddFeather(this);
            }

        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag(TagStrings.playerTag))
            {
                characterEvents.AddFeatherToPlayer.Invoke(MyFeatherKind);
                PlayerStatus.FeathersToCollectManager.RemoveFeather(this);
                this.gameObject.SetActive(false);
            }
        }


    }
}