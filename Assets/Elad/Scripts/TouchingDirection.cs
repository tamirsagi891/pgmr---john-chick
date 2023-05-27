using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchingDirection : MonoBehaviour
{
    private Vector2 wallCheckDirection => gameObject.transform.localScale.x > 0 ? Vector2.right : Vector2.left;
    private bool _isGrounded = true;

    public bool IsGrounded
    {
        get { return _isGrounded; }
        set
        {
            _animator.SetBool(AnimationStrings.isGrounded, value);
            _isGrounded = value;
        }
    }


    private bool _isOnWall = true;

    public bool IsOnWall
    {
        get { return _isOnWall; }
        set
        {
            _animator.SetBool(AnimationStrings.isOnWall, value);
            _isOnWall = value;
        }
    }

    private bool _isOnCeiling = true;

    public bool IsOnCeiling
    {
        get { return _isOnCeiling; }
        set
        {
            _animator.SetBool(AnimationStrings.isOnCeiling, value);
            _isOnCeiling = value;
        }
    }


    private bool _isOnPlatform;

    public bool IsOnPlatform
    {
        get => _isOnPlatform;
        set => _isOnPlatform = value;
    }

    public ContactFilter2D castFilterPlatform;


    public ContactFilter2D castFilter;
    [SerializeField] private float groundDistance = 0.05f;
    [SerializeField] private float wallDistance = 0.2f;
    [SerializeField] private float ceilingDistance = 0.05f;


    private RaycastHit2D[] groundHits = new RaycastHit2D[5];
    private RaycastHit2D[] wallHits = new RaycastHit2D[5];
    private RaycastHit2D[] ceilingHits = new RaycastHit2D[5];
    private RaycastHit2D[] platformHits = new RaycastHit2D[5];

    private Rigidbody2D _rB;

    private CircleCollider2D _circleCollider2D;
    private CapsuleCollider2D _capsuleCollider2D;
    private Animator _animator;

    [SerializeField] private LayerMask wallLayer;

    private void Awake()
    {
        _rB = GetComponent<Rigidbody2D>();
        _capsuleCollider2D = GetComponent<CapsuleCollider2D>();
        _circleCollider2D = GetComponent<CircleCollider2D>();
        _animator = GetComponent<Animator>();
    }


    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void FixedUpdate()
    {
        IsGrounded = _capsuleCollider2D.Cast(Vector2.down, castFilter, groundHits, groundDistance) > 0 || 
                     _circleCollider2D.Cast(Vector2.down, castFilter, groundHits, groundDistance) > 0;
        IsOnWall = _capsuleCollider2D.Cast(wallCheckDirection, castFilter, wallHits, wallDistance) > 0;
        IsOnCeiling = _capsuleCollider2D.Cast(Vector2.up, castFilter, ceilingHits, ceilingDistance) > 0;

        
        int numberOfPlatformHits =
            _circleCollider2D.Cast(Vector2.down, castFilterPlatform, platformHits, groundDistance);
        if (numberOfPlatformHits > 0)
        {
            for (int i = 0; i < numberOfPlatformHits; i++)
            {
                if (platformHits[i].collider != null && platformHits[i].collider.CompareTag("Platform"))
                {
                    
                    IsOnPlatform = true;
                    return;
                }
            }
        }

        IsOnPlatform = false;

    }
}