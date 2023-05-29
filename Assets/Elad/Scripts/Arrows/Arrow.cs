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
    [SerializeField][Tooltip("All arrows data are in the arrow data object")] private bool HowToAffectArrow;
    private int _damage;

    public int Damage
    {
        get => _damage;
        set => _damage = value;
    }


    private Vector2 _moveSpeed;

    public Vector2 MoveSpeed
    {
        get => _moveSpeed;
        set => _moveSpeed = value;
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

    
    private ArrowData _myArrowData;
    
    public ArrowData MyArrowData
    {
        get => _myArrowData;
        set
        {
            _myArrowData = value;
            MyArrowKind = _myArrowData.featherKind;
            Damage = _myArrowData.damage;
            MoveSpeed = _myArrowData.moveSpeed;
            LifeTime = _myArrowData.lifeTime;
            _rB.bodyType = _myArrowData.rigidbodyType2D;
            _addPlayerVelocity = _myArrowData.addPlayerVelocity;
        }
    }
    

    [Header("Times")] private float _lifeTime;
    public float LifeTime
    {
        get => _lifeTime;
        set => _lifeTime = value;
    }

    private bool _addPlayerVelocity;
    public bool AddPlayerVelocity
    {
        get => _addPlayerVelocity;
        set => _addPlayerVelocity = value;
    }

    

    private void Awake()
    {
        _rB = GetComponent<Rigidbody2D>();
        _controller = GetComponent<PlayerAttackController>();
        _playerAttack = PlayerStatus.player.GetComponent<PlayerAttack>();
    }

    private void Update()
    {
        LifeTime -= Time.deltaTime;
        if (LifeTime <= 0)
        {
            DestroyArrow();
        }
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
        }
        
        DestroyArrow();
    }

    private void DestroyArrow()
    {
        if (_playerAttack.UsePool)
        {
            bool inPoll = _playerAttack.ReturnArrowToPoll(this);
            if (!inPoll)
            {
                Destroy(gameObject);
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Fire()
    {
        int side = PlayerStatus.isFacingRight ? 1 : -1;
        Vector2 playerVelocity = PlayerStatus.playerVelocity;
        _rB.velocity = new Vector2((side * MoveSpeed.x) + playerVelocity.x, MoveSpeed.y + playerVelocity.y);
    }
}