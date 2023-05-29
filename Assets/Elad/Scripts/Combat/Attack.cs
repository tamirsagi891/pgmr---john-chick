using System;
using System.Collections;
using System.Collections.Generic;
using Mechanics.Enemies;
using UnityEngine;

[RequireComponent(typeof(PlayerAttackController))]
public class Attack : MonoBehaviour
{
    private Collider2D _collider2D;
    
    private PlayerAttackController _controller;

    private void Awake()
    {
        _collider2D = GetComponent<Collider2D>();
        _controller = GetComponent<PlayerAttackController>();
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        ICanBeAttacked damageable = other.GetComponentInParent<ICanBeAttacked>();
        if (damageable != null && !other.gameObject.CompareTag(TagStrings.playerTag))
        {
            _controller.Attack(damageable);
        }
    }
     
}
