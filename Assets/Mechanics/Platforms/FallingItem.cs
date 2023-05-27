using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(CircleCollider2D))]
public class FallingItem : MonoBehaviour
{
    public float maxDistanceFromPlayer = 20f; // the max distance the projectile can be from the player before being destroyed
    private Transform playerTransform;
    private FallingItemSpawner spawner;

    void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        spawner = GetComponentInParent<FallingItemSpawner>();
        GetComponent<Rigidbody2D>().gravityScale = 1; // Make sure the rigidbody falls down
    }

    void Update()
    {
        // If the spawner is deactivated or the projectile is too far from the player, deactivate the projectile
        if (!spawner.gameObject.activeInHierarchy || Vector2.Distance(transform.position, playerTransform.position) > maxDistanceFromPlayer)
        {
            gameObject.SetActive(false);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        string collisionTag = collision.gameObject.tag;

        if (collisionTag == "Player")
        {
            Debug.Log("Player");
            GetComponent<CircleCollider2D>().isTrigger = true;
        }
        else if (collisionTag != "Monster")
        {
            GetComponent<CircleCollider2D>().isTrigger = true;
        }
    }

    public void ResetProjectile()
    {
        // Resets the rigidbody velocity to 0
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        
        // Makes sure the collider is not a trigger
        GetComponent<CircleCollider2D>().isTrigger = false;
        
        // Deactivates the projectile
        gameObject.SetActive(false);
    }
}