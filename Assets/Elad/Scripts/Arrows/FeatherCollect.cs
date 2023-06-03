using System;
using Elad.Events;
using Elad.Save_Load_System;
using Elad.Scripts.Arrows;
using Unity.VisualScripting;
using UnityEngine;

namespace Elad.Scripts
{
    [RequireComponent(typeof(Collectable))]
    public class Feather : MonoBehaviour
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
                Destroy(gameObject); 
            }
            
            if (PlayerStatus.FeathersToCollectManager)
            {
                PlayerStatus.FeathersToCollectManager.Add(this);    
            }
            
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag(TagStrings.playerTag))
            {
                characterEvents.AddFeather.Invoke(MyFeatherKind);
                PlayerStatus.FeathersToCollectManager.Remove(this);
                
            }
        }
    }
}