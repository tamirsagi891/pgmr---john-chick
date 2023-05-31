using UnityEngine;
using System.Collections;

public class FallingPlatform : MonoBehaviour
{
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private float fallDelay = 1f;
    [SerializeField] private float respawnDelay = 5f;
    [SerializeField] private float distanceFromPlayerToDeactivate = 20f;

    private BoxCollider2D boxCollider;
    private Rigidbody2D rb;
    private Transform playerTransform;

    private Vector2 originalPosition;
    private bool isFalling = false;

    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        originalPosition = transform.position;
        playerTransform = GameObject.FindGameObjectWithTag(playerTag).transform;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag(playerTag) && !isFalling)
        {
            StartCoroutine(ShakeAndFall());
        }
    }

    private IEnumerator ShakeAndFall()
    {
        isFalling = true;

        // Shake
        float elapsedTime = 0f;
        while (elapsedTime < fallDelay)
        {
            transform.position = new Vector2(transform.position.x + Mathf.Sin(elapsedTime * 50) * 0.01f, transform.position.y);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Fall
        boxCollider.isTrigger = true;
        rb.bodyType = RigidbodyType2D.Dynamic;

        yield return new WaitForSeconds(respawnDelay);

        // Reset
        ResetPlatform();
    }

    private void ResetPlatform()
    {
        rb.velocity = Vector2.zero;
        transform.position = originalPosition;
        boxCollider.isTrigger = false;
        rb.bodyType = RigidbodyType2D.Static;
        isFalling = false;
    }

    private void Update()
    {
        if (isFalling && Vector2.Distance(playerTransform.position, transform.position) > distanceFromPlayerToDeactivate)
        {
            gameObject.SetActive(false);
        }
    }

    private void OnEnable()
    {
        if (isFalling)
        {
            ResetPlatform();
        }
    }
}
