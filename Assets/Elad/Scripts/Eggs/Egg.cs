using System;
using System.Collections;
using System.Collections.Generic;
using Elad.Scripts;
using Mechanics.Enemies;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(PlayerAttackController))]
public class Egg : MonoBehaviour
{
    [SerializeField] [Tooltip("All Eggs data are in the Egg data object")]
    private bool HowToAffectEgg;

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

    private EggsManager.EggKind _myEggKind;

    private PlayerAttackController _controller;
    private Rigidbody2D _rB;

    private EggsManager _eggsManager;

    public EggsManager eggsManager
    {
        set => _eggsManager = value;
    }

    public EggsManager.EggKind MyEggKind
    {
        get => _myEggKind;
        set => _myEggKind = value;
    }


    private EggData _myEggData;
    
    public EggData MyEggData
    {
        get => _myEggData;
        set
        {
            _myEggData = value;
            MyEggKind = _myEggData.eggKind;
            Damage = _myEggData.damage;
            MoveSpeed = _myEggData.moveSpeed;
            LifeTime = _myEggData.lifeTime;
            _rB.bodyType = _myEggData.rigidbodyType2D;
            _addPlayerVelocity = _myEggData.addPlayerVelocity;
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
    }

    private void Update()
    {
        LifeTime -= Time.deltaTime;
        if (LifeTime <= 0)
        {
            DestroyEgg();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the other gameobject's layer is contained within the hitFilter layerMask
        if (_myEggData.hitFilter.layerMask == (_myEggData.hitFilter.layerMask | (1 << other.gameObject.layer)))
        {
            ICanBeAttacked damageable = other.GetComponentInParent<ICanBeAttacked>();
            if (damageable != null)
            {
                _controller.Attack(damageable);
            }

            DestroyEgg();
        }
    }


    private void DestroyEgg()
    {
        if (_eggsManager.UsePoolEgg)
        {
            bool inPoll = _eggsManager.ReturnEggToPoll(this);
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

        var velocityY = playerVelocity.y > 0 ? MoveSpeed.y : MoveSpeed.y + playerVelocity.y;
        _rB.velocity = new Vector2(MoveSpeed.x + playerVelocity.x, velocityY);
    }
}