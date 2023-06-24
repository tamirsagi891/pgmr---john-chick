using Elad.Events;
using UnityEngine;
using Logger = Nemesh.Logger;
using FMODUnity;

namespace Elad.Scripts.Arrows
{
    [RequireComponent(typeof(Collectable), typeof(StudioEventEmitter))]
    public class FeatherToCollect : MonoBehaviour
    {
        [SerializeField] private FeathersManager.FeatherKind myFeatherKind;
        private int _id = 0;

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

        public int ID
        {
            get => _id;
            set => _id = value;
        }

        [Header("Sounds")] private StudioEventEmitter _emitter;

        private void Start()
        {
            if (PlayerStatus.InitializeFromJason)
            {
                // Destroy(gameObject);
            }
            else if (PlayerStatus.FeathersToCollectManager)
            {
                PlayerStatus.FeathersToCollectManager.AddFeather(this);
            }

            if (AudioManager.instance)
            {
                _emitter = AudioManager.instance.InitializeEventEmitter(FMODEvents.instance.idleFeatherSound,
                    gameObject);
                _emitter.Play();
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag(TagStrings.playerTag))
            {
                if (AudioManager.instance)
                {
                    _emitter.Stop();
                    AudioManager.instance.PlayOneShot(FMODEvents.instance.collectFeatherSound, transform.position);
                }

                characterEvents.AddFeatherToPlayer.Invoke(MyFeatherKind);
                PlayerStatus.FeathersToCollectManager.RemoveFeather(this);
                this.gameObject.SetActive(false);
            }
        }
    }
}