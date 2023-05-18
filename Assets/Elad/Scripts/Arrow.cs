using System;
using System.Collections;
using System.Collections.Generic;
using Elad.Scripts;
using Mechanics.Enemies;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(PlayerAttackController))]
public class Arrow : MonoBehaviour
{
    private int damage = 5;
    public int Damage
    {
        get => damage;
        set => damage = value;
    }
    
    
    [SerializeField] private Vector2 moveSpeed = new Vector2(3f, 0);
    public Vector2 MoveSpeed
    {
        get => moveSpeed;
        set => moveSpeed = value;
    }

    private FeathersManager.FeatherKind _myArrowKind;

    private PlayerAttackController _controller;
    private Rigidbody2D _rB;

    private PlayerAttack _playerAttack;

    public PlayerAttack PlayerAttack
    {
        set => _playerAttack = value;
    }

    public FeathersManager.FeatherKind MyArrowKind
    {
        get => _myArrowKind;
        set => _myArrowKind = value;
    }


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

            bool inPoll = _playerAttack.ReturnArrowToPoll(this);
            if (!inPoll)
            {
                Destroy(gameObject);
            }
        }
    }

    public void Fire()
    {
        int side = PlayerStatus.isFacingRight ? 1 : -1;
        _rB.velocity = new Vector2(side * MoveSpeed.x, MoveSpeed.y);
    }
}