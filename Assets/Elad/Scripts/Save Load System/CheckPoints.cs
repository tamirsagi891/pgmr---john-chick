using System;
using System.Threading;
using Elad.Events;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using Logger = Nemesh.Logger;
using Random = UnityEngine.Random;

namespace Elad.Scripts.Save_Load_System
{
    public class CheckPoints : MonoBehaviour
    {

        private const string CHECKPOINT_TEXT_OFF = "Activate Chickpoint \n<sprite tint=1 name=downArrow>";
        private const string CHECKPOINT_TEXT_ON = "Update Chickpoint \n<sprite tint=1 name=downArrow>";
        
        [SerializeField] private bool _isInvisibleCheckPoint = false;
        [SerializeField] private GameObject chickenPrefab;
        [SerializeField] private TextMeshPro checkPointTextBox; 
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

                if (!isOn)
                {
                    SpawnChickens();
                    if (checkPointTextBox != null)
                    {
                        checkPointTextBox.text = CHECKPOINT_TEXT_ON;
                    }
                }
                
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
                if (isOn)
                {
                    DestroyChickens();
                    if (checkPointTextBox != null)
                    {
                        checkPointTextBox.text = CHECKPOINT_TEXT_OFF;
                    }
                }
                isOn = false;
                if (animator)
                {
                    animator.SetBool("isOn", false);
                }
            }
        }

        private void SpawnChickens()
        {
            if (_isInvisibleCheckPoint ) return;
            int numChickens = Random.Range(5, 9);
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