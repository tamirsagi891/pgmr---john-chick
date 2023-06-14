using Elad.Scripts.Combat;
using UnityEngine;

[RequireComponent(typeof(CompositeCollider2D))]
public class Spikes : MonoBehaviour
{
    [SerializeField] private int damage = 1; // Damage that the spikes will cause on collision with the player
    [SerializeField] private float knockBackForce = 3f; // Force with which the player will be knocked back

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Damageable damageable = collision.gameObject.GetComponent<Damageable>();
            if (damageable != null)
            {
                // Determine knockBack direction based on the player's facing direction
                damageable.GotHit(damage, knockBackForce * Vector2.up);
            }
        }
    }
}