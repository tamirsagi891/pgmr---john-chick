// using System;
// using System.Collections;
// using System.Collections.Generic;
// using Elad.Scripts;
// using Mechanics.Enemies;
// using Unity.VisualScripting;
// using UnityEngine;
//
// [RequireComponent(typeof(PlayerAttackController))]
// public class Egg : MonoBehaviour
// {
//     [SerializeField][Tooltip("All Eggs data are in the Egg data object")] private bool HowToAffectEgg;
//     private int _damage;
//
//     public int Damage
//     {
//         get => _damage;
//         set => _damage = value;
//     }
//
//
//     private Vector2 _moveSpeed;
//
//     public Vector2 MoveSpeed
//     {
//         get => _moveSpeed;
//         set => _moveSpeed = value;
//     }
//
//     private FeathersManager.FeatherKind _myEggKind;
//
//     private PlayerAttackController _controller;
//     private Rigidbody2D _rB;
//
//     private FeathersManager _playerAttack;
//
//     public FeathersManager FeathersManager
//     {
//         set => _playerAttack = value;
//     }
//
//     public FeathersManager.FeatherKind MyEggKind
//     {
//         get => _myEggKind;
//         set => _myEggKind = value;
//     }
//
//     
//     private EggData _myEggData;
//     
//     public EggData MyEggData
//     {
//         get => _myEggData;
//         set
//         {
//             _myEggData = value;
//             MyEggKind = _myEggData.featherKind;
//             Damage = _myEggData.damage;
//             MoveSpeed = _myEggData.moveSpeed;
//             LifeTime = _myEggData.lifeTime;
//             _rB.bodyType = _myEggData.rigidbodyType2D;
//             _addPlayerVelocity = _myEggData.addPlayerVelocity;
//         }
//     }
//     
//
//     [Header("Times")] private float _lifeTime;
//     public float LifeTime
//     {
//         get => _lifeTime;
//         set => _lifeTime = value;
//     }
//
//     private bool _addPlayerVelocity;
//     public bool AddPlayerVelocity
//     {
//         get => _addPlayerVelocity;
//         set => _addPlayerVelocity = value;
//     }
//
//     
//
//     private void Awake()
//     {
//         _rB = GetComponent<Rigidbody2D>();
//         _controller = GetComponent<PlayerAttackController>();
//         _playerAttack = PlayerStatus.player.GetComponent<FeathersManager>();
//     }
//
//     private void Update()
//     {
//         LifeTime -= Time.deltaTime;
//         if (LifeTime <= 0)
//         {
//             DestroyEgg();
//         }
//     }
//
//     private void OnTriggerEnter2D(Collider2D other)
//     {
//         if (!other.CompareTag(TagStrings.playerTag))
//         {
//             ICanBeAttacked damageable = other.GetComponentInParent<ICanBeAttacked>();
//             if (damageable != null)
//             {
//                 _controller.Attack(damageable);
//             }
//         }
//         
//         DestroyEgg();
//     }
//
//     private void DestroyEgg()
//     {
//         if (_playerAttack.UsePoolArrow)
//         {
//             bool inPoll = _playerAttack.ReturnEggToPoll(this);
//             if (!inPoll)
//             {
//                 Destroy(gameObject);
//             }
//         }
//         else
//         {
//             Destroy(gameObject);
//         }
//     }
//
//     public void Fire()
//     {
//         int side = PlayerStatus.isFacingRight ? 1 : -1;
//         Vector2 playerVelocity = PlayerStatus.playerVelocity;
//         _rB.velocity = new Vector2((side * MoveSpeed.x) + playerVelocity.x, MoveSpeed.y + playerVelocity.y);
//     }
// }
