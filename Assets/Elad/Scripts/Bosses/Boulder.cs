using System;
using System.Collections;
using System.Collections.Generic;
using Elad.Scripts;
using Elad.Scripts.Combat;
using UnityEngine;
using Random = UnityEngine.Random;

public class Boulder : MonoBehaviour
{
    private Transform target; 
    [SerializeField] private float startSpeed = 10f;
    [SerializeField][Range(0,1)] private float playerVelocityMult = 0.5f;
    [SerializeField] private float maxSpeed = 20f; // Define your maximum speed here
    [SerializeField] private float accelerationTime = 5f; // Time it takes to reach max speed
    [SerializeField] private float destroyTime = 5f;
    [SerializeField] private Vector2 knockBack = Vector2.right;
    [SerializeField] private float knockBackDelay = 0.1f;
    [SerializeField] private float startRotationSpeed = 90f; // Degrees per second
    private float accelerationRate;
    private Rigidbody2D rb;
    private float currentSpeed;

    private void Awake()
    {
        target = PlayerStatus.Player.transform;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Vector2 direction = (target.position - transform.position).normalized;
        if (direction.y > 0) return;
        float playerXVelocity = PlayerStatus.Player.GetComponent<Rigidbody2D>().velocity.x;
        

        
        rb.velocity = direction * startSpeed;
        int rand = Random.Range(0, 2);
        rb.velocity = new Vector2(rb.velocity.x + (playerXVelocity * playerVelocityMult * rand), rb.velocity.y);

        // rb.velocity = new Vector2(rb.velocity.x * playerXVelocity, rb.velocity.y);
        
        var rotationMult = rb.velocity.x > 0 ? -1 : 1;
        rb.angularVelocity = rotationMult * startRotationSpeed;

        currentSpeed = startSpeed;
        accelerationRate = (maxSpeed - startSpeed) / accelerationTime;
    }
    
    private void Update()
    {
        destroyTime -= Time.deltaTime;
        if (destroyTime <= 0)
        {
            Destroy(gameObject);
        }

        // Increase the speed over time
        currentSpeed += accelerationRate * Time.deltaTime;
        rb.velocity = rb.velocity.normalized * currentSpeed;

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(TagStrings.playerTag))
        {
            Damageable damageablePlayer = PlayerStatus.PlayerDamageable;
            var knockBackMult = rb.velocity.x > 0 ? 1 : -1;
            var curKnockBack = knockBack * knockBackMult;
            damageablePlayer.GotHit(1, curKnockBack, knockBackDelay);
            AudioManager.instance.PlayOneShot(FMODEvents.instance.rockHit, transform.position);

            Destroy(gameObject);

        }
    }
}
