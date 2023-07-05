using BitStrap;
using Elad.Scripts.Combat;
using Mechanics.Enemies;
using UnityEngine;
using Logger = Nemesh.Logger;

[RequireComponent(typeof(CompositeCollider2D))]
public class Spikes : MonoBehaviour, IAttacker
{
    [SerializeField] private int damage = 1; // Damage that the spikes will cause on collision with the player
    [SerializeField] private float knockBackForce = 3f; // Force with which the player will be knocked back
    [SerializeField]
    [TagSelector]
    private string enemyTag = "Enemy";
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent(out ICanBeAttacked target))
        {
            Attack(target);
            return;
        }

        if (collision.CompareTag(enemyTag))
        {
            var enemy = collision.gameObject.GetComponentInParent<ICanBeAttacked>();
            Attack(enemy);
            return;
        }
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

    public bool Attack(ICanBeAttacked attackTarget)
    {
        return attackTarget.Hurt(GetAttackParameters());
    }

    public AttackParameters GetAttackParameters()
    {
        return new AttackParameters(
            attacker: this,
            damage: damage,
            knockBack: knockBackForce * Vector2.up,
            type: AttackType.Regular);
    }
}