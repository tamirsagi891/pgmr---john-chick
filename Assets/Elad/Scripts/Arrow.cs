using System;
using System.Collections;
using System.Collections.Generic;
using Mechanics.Enemies;
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

    
    // Start is called before the first frame update
    void Start()
    {
        _rB.velocity = new Vector2(moveSpeed.x * transform.localScale.x, moveSpeed.y);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        ICanBeAttacked damageable = other.GetComponentInParent<ICanBeAttacked>();
        if (damageable != null)
        {
            _controller.Attack(damageable);
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
