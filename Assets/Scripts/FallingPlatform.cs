using UnityEngine;
using System.Collections;
using Elad.Scripts;
using FMOD.Studio;
using FMODUnity;
using STOP_MODE = FMOD.Studio.STOP_MODE;

public class FallingPlatform : MonoBehaviour
{
    
    
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private float fallDelay = 1f;
    [SerializeField] private float respawnDelay = 5f;
    [SerializeField] private float popInTime = 0.5f;
    [SerializeField] private float distanceFromPlayerToDeactivate = 20f;
    [SerializeField] private float shakeIntensity = 0.01f;

    [SerializeField]
    private bool changeToTriggerOnFall;

    private BoxCollider2D boxCollider;
    private Rigidbody2D rb;
    private Transform playerTransform;

    private Vector2 originalPosition;
    private Vector3 originalScale;
    private bool isFalling = false;
    private bool isReadyToReset = false;
    private float respawnTimer = 0f;

    [Header("Sounds")] private EventInstance crumblingPlatform;
    private StudioEventEmitter _emitter;
    [SerializeField] private float emitTime = 1f;
    private float _emitTimer = 1f;
    private bool _emitting;

    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        originalPosition = transform.position;
        originalScale = transform.localScale;
        playerTransform = GameObject.FindGameObjectWithTag(playerTag).transform;
    }

    private void Start()
    {
        _emitter = GetComponent<StudioEventEmitter>();
        if (AudioManager.instance)
        {
            // _emitter = AudioManager.instance.InitializeEventEmitter(FMODEvents.instance.returnPlatform, gameObject);    
        }
        
        crumblingPlatform = AudioManager.instance.CreatEventInstance(FMODEvents.instance.crumblingPlatform);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag(playerTag) && !isFalling)
        {
            if (collision.contacts[0].normal.y < 0) // If the player is above the platform
            {
                PlayerStatus.isOnWoodPlatform = true;
                crumblingPlatform.start();
                StartCoroutine(ShakeAndFall());
            }
        }
    }

    private IEnumerator ShakeAndFall()
    {
        isFalling = true;

        // Shake
        float elapsedTime = 0f;
        while (elapsedTime < fallDelay)
        {
            transform.position = new Vector2(transform.position.x + Mathf.Sin(elapsedTime * 50) * shakeIntensity, transform.position.y);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        if (changeToTriggerOnFall)
        {
            boxCollider.isTrigger = true;
        }
        // Fall
        crumblingPlatform.stop(STOP_MODE.ALLOWFADEOUT);
        AudioManager.instance.PlayOneShot(FMODEvents.instance.fallingPlatform, transform.position);
        rb.bodyType = RigidbodyType2D.Dynamic;
    }

    private void ResetPlatform()
    {
        transform.position = originalPosition;
        boxCollider.isTrigger = false;
        rb.bodyType = RigidbodyType2D.Static;
        isFalling = false;
        isReadyToReset = false;
        respawnTimer = 0f;

        // Pop-in effect
        StartCoroutine(PopIn());
    }

    private IEnumerator PopIn()
    {
        float elapsedTime = 0f;
        transform.localScale = Vector3.zero;

        while (elapsedTime < popInTime)
        {
            transform.localScale = Vector3.Lerp(Vector3.zero, originalScale, elapsedTime / popInTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        _emitter.Play();
        _emitting = true;
        _emitTimer = emitTime;
        transform.localScale = originalScale;
    }

    private void Update()
    {
        if (_emitting)
        {
            _emitTimer -= Time.deltaTime;
            if (_emitTimer <= 0)
            {
                _emitting = false;
                _emitter.Stop();
            }
        }
        
        if (isFalling && Vector2.Distance(playerTransform.position, transform.position) > distanceFromPlayerToDeactivate)
        {
            isReadyToReset = true;
        }

        if (isReadyToReset)
        {
            respawnTimer += Time.deltaTime;
            if (respawnTimer >= respawnDelay)
            {
                ResetPlatform();
            }
        }
    }
}
