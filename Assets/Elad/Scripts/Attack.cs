using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    private Collider2D _collider2D;
    [SerializeField] private int attackAmount = 10;
    [SerializeField] private Vector2 knockBack = Vector2.zero;

    private void Awake()
    {
        _collider2D = GetComponent<Collider2D>();
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        Damageable damageable = other.GetComponent<Damageable>();
        if (damageable)
        {
            Vector2 finalKnockBack =
                transform.parent.localScale.x > 0 ? knockBack : new Vector2(-knockBack.x, knockBack.y); 
            bool gotHit = damageable.Hit(attackAmount, finalKnockBack);
        }
    }
     
}
