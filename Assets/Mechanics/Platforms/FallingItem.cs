using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(CircleCollider2D))]
public class FallingItem : MonoBehaviour
{
    [SerializeField] private int damage = 1; // Damage that the item will cause on collision with the player
    [SerializeField] private float knockBackForce = 5f; // Force with which the player will be knocked back
    [SerializeField] private float maxDistanceFromPlayer = 50f; // The max distance the projectile can be from the player before being deactivated

    private Transform playerTransform;
    private FallingItemSpawner spawner;

    private void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        spawner = GetComponentInParent<FallingItemSpawner>();
        GetComponent<Rigidbody2D>().gravityScale = 1; // Make sure the rigidbody falls down
    }

    private void Update()
    {
        // If the spawner is deactivated or the projectile is too far from the player, deactivate the projectile
        if (!spawner.gameObject.activeInHierarchy || Vector2.Distance(transform.position, playerTransform.position) > maxDistanceFromPlayer)
        {
            gameObject.SetActive(false);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        string collisionTag = collision.gameObject.tag;

        if (collisionTag == "Player")
        {
            Damageable damageable = collision.gameObject.GetComponent<Damageable>();
            if (damageable != null)
            {
                Vector2 knockBack = (collision.transform.position.x > transform.position.x) ? Vector2.right : Vector2.left;
                damageable.GotHit(damage, knockBack * knockBackForce);
            }
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
