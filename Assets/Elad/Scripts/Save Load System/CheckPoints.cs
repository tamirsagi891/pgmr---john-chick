using System;
using System.Threading;
using Elad.Events;
using UnityEngine;
using UnityEngine.InputSystem;
using Logger = Nemesh.Logger;
using Random = UnityEngine.Random;

namespace Elad.Scripts.Save_Load_System
{
    public class CheckPoints : MonoBehaviour
    {
        [SerializeField] private bool _isInvisibleCheckPoint = false;
        [SerializeField] private GameObject chickenPrefab;
        private Vector3 _position;
        private bool isOn = false;
        private GameObject[] chickens;
        private Animator animator;
        private FadeText _fadeText;


        [SerializeField] [Tooltip("The amount of time to vanish the instructions after using the check point")]
        private float hideInstructionsTime;

        private float _hideInstructionsTimer;
        private bool _hideInstructions;

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
            _fadeText = GetComponentInChildren<FadeText>();

            if (!_isInvisibleCheckPoint)
            {
                DestroyChickens();
            }
            else
            {
                PlayerStatus.LastCheckPoint = this;
            }
        }

        private void Update()
        {
            if (_hideInstructions)
            {
                _hideInstructionsTimer -= Time.deltaTime;
                if (_hideInstructionsTimer <= 0)
                {
                    Logger.Log("Now you can see the instructions");
                    _hideInstructions = false;
                    _fadeText.gameObject.SetActive(true);
                }
            }
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
            if (PlayerStatus.LastCheckPoint == this)
            {
                if (_fadeText)
                {
                    _fadeText.gameObject.SetActive(false);
                    _hideInstructions = true;
                    _hideInstructionsTimer = hideInstructionsTime;
                }

                SpawnChickens();
                isOn = true;
                if (animator)
                {
                    animator.SetBool("isOn", true);
                }
            }
            else
            {
                // Open checkpoint
                if (_fadeText)
                    _fadeText.gameObject.SetActive(true);
                isOn = false;
                DestroyChickens();
                if (animator)
                {
                    animator.SetBool("isOn", false);
                }
            }
        }

        private void SpawnChickens()
        {
            if (_isInvisibleCheckPoint ) return;
            int numChickens = Random.Range(4, 7);
            chickens = new GameObject[numChickens];
            for (int i = 0; i < numChickens; i++)
            {
                chickens[i] = Instantiate(chickenPrefab, transform.position, Quaternion.identity, transform);
            }
        }

        private void DestroyChickens()
        {
            if (_isInvisibleCheckPoint) return;
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