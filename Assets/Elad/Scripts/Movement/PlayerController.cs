using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using BitStrap;
using Elad.Events;
using Elad.Scripts;
using FMODUnity;
using Managers;
using Mechanics.UI.Menus;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using Logger = Nemesh.Logger;

[RequireComponent(typeof(Rigidbody2D), typeof(TouchingDirection))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private bool isGamePaused;

    [Space(10)] [Header("Movement")] [SerializeField]
    private float airWalkSpeed = 3f;

    [SerializeField] private float walkSpeed = 5f;
    private Vector2 _movementInput;
    private bool _isMoving;

    public bool IsMoving
    {
        get => _isMoving;
        set
        {
            _isMoving = value;
            _animator.SetBool(AnimationStrings.isMoving, value);
        }
    }

    [Space(10)] [Header("Touching")] private TouchingDirection _touchingDirection;


    [Space(10)] [Header("Components")] private SpriteRenderer _sP;
    private Rigidbody2D _rB;
    private Animator _animator;

    [Space(10)] [Header("Collider")] private CapsuleCollider2D _capsuleCollider2D;
    private CircleCollider2D _circleCollider2D;

    private HorizontalMovement _horizontalMovementPlayer;

    public enum ColliderKind
    {
        Capsule,
        Circle,
        DodgeRoll
    }

    [Space(10)] [Header("Wall Movement")] [SerializeField]
    private bool _isWallSliding;


    private bool _isInWallJump;
    private bool _wallJump;

    private float _wallJumpingTimer;

    private void OnEnable()
    {
        characterEvents.PlayerDied.AddListener(OnPlayerDied);
        characterEvents.PlayerRevive.AddListener(OnPlayerRevive);
    }

    private void OnDisable()
    {
        characterEvents.PlayerDied.RemoveListener(OnPlayerDied);
        characterEvents.PlayerRevive.RemoveListener(OnPlayerRevive);
    }

    private void Awake()
    {
        PlayerStatus.Player = this.gameObject;
        PlayerStatus.PlayerController = this;
        _rB = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _touchingDirection = GetComponent<TouchingDirection>();
        _capsuleCollider2D = GetComponent<CapsuleCollider2D>();
        _circleCollider2D = GetComponent<CircleCollider2D>();
        ChangeCollider(ColliderKind.Capsule);
        _horizontalMovementPlayer = GetComponent<HorizontalMovement>();
        _sP = GetComponent<SpriteRenderer>();
    }

    

    public bool CanMove
    {
        get { return _animator.GetBool(AnimationStrings.canMove); }
    }

    public bool IsAlive
    {
        get { return _animator.GetBool(AnimationStrings.isAlive); }
    }

    private void Update()
    {
        isGamePaused = GeneralGameManager.IsGamePause;
    }

    public void OpenEndUpMenu()
    {
        MenuManager.Menu.OpenEndLevelMenu();
    }

    public void ChangeCollider(ColliderKind colliderKind)
    {
        switch (colliderKind)
        {
            case ColliderKind.Capsule:
                _capsuleCollider2D.isTrigger = false;
                // _circleCollider2D.enabled = false;
                break;

            case ColliderKind.Circle:
                _capsuleCollider2D.isTrigger = true;
                // _circleCollider2D.enabled = true;
                break;

            case ColliderKind.DodgeRoll:
                break;
        }
    }

    public bool CantGetInput()
    {
        var retVal = GeneralGameManager.IsGamePause || !PlayerStatus.IsAlive || PlayerStatus.InCutScene;
        return retVal;
    }

    [Button]
    public void CloseMovement()
    {
        _horizontalMovementPlayer.DirectionX = 0;
        IsMoving = false;
    }

    private void OnPlayerDied()
    {
        ChangeSortingLayerToDefault();
        _rB.constraints = RigidbodyConstraints2D.FreezeAll;
    }

    private void OnPlayerRevive()
    {
        ChangeSortingLayerToTileMap();
        _rB.constraints = RigidbodyConstraints2D.None;
        _rB.constraints = RigidbodyConstraints2D.FreezeRotation;
    }
    

    [Button]
    public void ChangeSortingLayerToDefault()
    {
        _sP.sortingLayerName  = SortingOrderStrings.defaultSortingLayer;
    }
    
    [Button]
    public void ChangeSortingLayerToTileMap()
    {
        _sP.sortingLayerName  = SortingOrderStrings.tileMapSortingLayer;
    }
}