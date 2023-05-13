using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

[RequireComponent(typeof(Rigidbody2D), typeof(TouchingDirection))]
public class PlayerController : MonoBehaviour
{
    
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



    [Space(10)] [Header("Components")] private Rigidbody2D _rB;
    private Animator _animator;

    [Space(10)] [Header("Collider")] private CapsuleCollider2D _capsuleCollider2D;
    private CircleCollider2D _circleCollider2D;

    public enum ColliderKind
    {
        Capsule,
        Circle,
        DodgeRoll
    }

    [Space(10)] [Header("Wall Movement")] [SerializeField]
    private bool _isWallSliding;

    [SerializeField] private float wallSlidingSpeed = 2f;

    private bool _isInWallJump;
    private bool _wallJump;
    [SerializeField] private Vector2 wallJumpingPower = new Vector2(8f, 4f);
    [SerializeField] private float wallJumpingTime = 0.3f;
    private float _wallJumpingTimer;

    private float CurrentMoveSpeed
    {
        get
        {
            if (CanMove)
            {
                
                if (_isMoving && !_touchingDirection.IsOnWall)
                {
                    if (_touchingDirection.IsGrounded)
                    {
                       
                    }


                    else
                    {
                        return 0;
                    }
                }
                else
                {
                    //Movement lock
                    return 0;
                }
            }

            return 0;
        }
    }


    private void Awake()
    {
        _rB = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _touchingDirection = GetComponent<TouchingDirection>();
        _capsuleCollider2D = GetComponent<CapsuleCollider2D>();
        _circleCollider2D = GetComponent<CircleCollider2D>();
        ChangeCollider(ColliderKind.Capsule);
    }
    
    
    
    public bool CanMove
    {
        get { return _animator.GetBool(AnimationStrings.canMove); }
    }
    
    public bool IsAlive
    {
        get { return _animator.GetBool(AnimationStrings.isAlive); }
    }

    

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            _animator.SetTrigger(AnimationStrings.attackTrigger);
            _animator.SetTrigger(AnimationStrings.attackKickLowTrigger);
        }
    }
    

    public void ChangeCollider(ColliderKind colliderKind)
    {
        switch (colliderKind)
        {
            case ColliderKind.Capsule:
                _capsuleCollider2D.enabled = true;
                _circleCollider2D.enabled = false;
                break;

            case ColliderKind.Circle:
                _capsuleCollider2D.enabled = false;
                _circleCollider2D.enabled = true;
                break;

            case ColliderKind.DodgeRoll:
                break;
        }
    }
}