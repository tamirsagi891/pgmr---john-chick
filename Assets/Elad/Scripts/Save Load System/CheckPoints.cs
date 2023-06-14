using Elad.Events;
using UnityEngine;
using UnityEngine.InputSystem;
using Logger = Nemesh.Logger;

namespace Elad.Scripts.Save_Load_System
{
    public class CheckPoints : MonoBehaviour
    {
        [SerializeField] private GameObject chickenPrefab;
        private Vector3 _position;
        private bool isOn = false;
        private GameObject[] chickens;
        private Animator animator;

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
            animator = GetComponent<Animator>();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag(TagStrings.playerTag))
            {
                PlayerStatus.PlayerInsideCheckPoint = true;
                PlayerStatus.LastCheckPoint = this;

                if (isOn)
                {
                    // Spawn chickens
                    SpawnChickens();
                }
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag(TagStrings.playerTag))
            {
                PlayerStatus.PlayerInsideCheckPoint = false;

                // Destroy chickens
                DestroyChickens();
            }
        }

        private void OpenOrCloseCheckPoint()
        {
            if (!PlayerStatus.SaveGameManager.FirstTime)
            {
                if (PlayerStatus.LastCheckPoint == this)
                {
                    // Close checkpoint
                    isOn = false;
                    SpawnChickens();
                    animator.SetBool("isOn", true);
                }
                else
                {
                    // Open checkpoint
                    isOn = true;
                    //DestroyChickens();
                    animator.SetBool("isOn", false);
                }
            }
            else
            {
                PlayerStatus.SaveGameManager.FirstTime = false;
            }
        }

        private void SpawnChickens()
        {
            int numChickens = Random.Range(4, 7);
            chickens = new GameObject[numChickens];
            for (int i = 0; i < numChickens; i++)
            {
                chickens[i] = Instantiate(chickenPrefab, transform.position, Quaternion.identity);
            }
        }

        private void DestroyChickens()
        {
            if (chickens != null)
            {
                foreach (GameObject chicken in chickens)
                {
                    Destroy(chicken);
                }
            }
        }
    }
}