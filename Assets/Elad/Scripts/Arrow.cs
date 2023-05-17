using System;
using System.Collections;
using System.Collections.Generic;
using Mechanics.Enemies;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(PlayerAttackController))]
public class Arrow : MonoBehaviour
{
    
    [SerializeField] private int damage = 5;
    [SerializeField] private Vector2 moveSpeed = new Vector2(3f, 0);

    private PlayerAttackController _controller;
    private Rigidbody2D _rB;

    

    private void Awake()
    {
        _rB = GetComponent<Rigidbody2D>();
        _controller = GetComponent<PlayerAttackController>();

    }

    

    private void OnTriggerEnter2D(Collider2D other)
    {
        
        

        if (!other.CompareTag(TagStrings.playerTag))
        { 
            ICanBeAttacked damageable = other.GetComponentInParent<ICanBeAttacked>();
            if (damageable != null)
            {
                _controller.Attack(damageable);
            }
        
            Destroy(gameObject);    
        }
        
    }

    public void Fire()
    {
        _rB.velocity = new Vector2(moveSpeed.x * transform.localScale.x, moveSpeed.y); 
    }
    
}
