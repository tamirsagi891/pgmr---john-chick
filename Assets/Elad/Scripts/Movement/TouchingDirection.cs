using System;
using System.Collections;
using System.Collections.Generic;
using BitStrap;
using UnityEngine;

public class TouchingDirection : MonoBehaviour
{
    private Vector2 wallCheckDirection => gameObject.transform.localScale.x > 0 ? Vector2.right : Vector2.left;
    [SerializeField]
    [ReadOnly]
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
    

    private RaycastHit2D[] groundHits = new RaycastHit2D[5];
    private RaycastHit2D[] wallHits = new RaycastHit2D[5];
    private RaycastHit2D[] ceilingHits = new RaycastHit2D[5];
    private RaycastHit2D[] platformHits = new RaycastHit2D[5];

    private CircleCollider2D _circleCollider2D;
    private CapsuleCollider2D _capsuleCollider2D;
    private Animator _animator;

    [Header("Layer's to include the rays")]
    [SerializeField] private ContactFilter2D castFilterPlatform;
    [SerializeField] private ContactFilter2D castFilterGround;
    [SerializeField] private ContactFilter2D castFilterWall;

    [Space(10)][Header("Distance of cast ray")] [SerializeField]
    private float groundDistance = 0.05f;

    [SerializeField] private float wallDistance = 0.2f;
    [SerializeField] private float ceilingDistance = 0.05f;

    [Space(10)] [Header("Offsets for the rays")] [SerializeField]
    private Vector2 groundOffset = new Vector2(0f, 0f);

    [SerializeField] private float groundGapAmount = 0.1f;
    [SerializeField]
    private Vector2 wallOffset = new Vector2(0f, 0f);
    
    private void Awake()
    {
        _capsuleCollider2D = GetComponent<CapsuleCollider2D>();
        _circleCollider2D = GetComponent<CircleCollider2D>();
        _animator = GetComponent<Animator>();
    }


    private void FixedUpdate()
    {
        GroundAndPlatformCast();
        WallCast();

        IsOnCeiling = _capsuleCollider2D.Cast(Vector2.up, castFilterGround, ceilingHits, ceilingDistance) > 0;
    }


    private void GroundAndPlatformCast()
    {
        // Calculate the position at the bottom of the circle collider
        Vector2 castOrigin = _circleCollider2D.bounds.center;
        castOrigin.y -= _circleCollider2D.bounds.extents.y;
        castOrigin += groundOffset;


        Vector2 castLeft = castOrigin - new Vector2(groundGapAmount/2, 0);
        Vector2 casRight = castOrigin + new Vector2(groundGapAmount/2, 0);
        
        
        RaycastHit2D hitGroundLeft =
            Physics2D.Raycast(castLeft, Vector2.down, groundDistance, castFilterGround.layerMask);
        
        RaycastHit2D hitGroundRight =
            Physics2D.Raycast(casRight, Vector2.down, groundDistance, castFilterGround.layerMask);
        
        // Draw debug line for the cast
        Debug.DrawRay(castLeft, Vector2.down * groundDistance, Color.red);
        Debug.DrawRay(casRight, Vector2.down * groundDistance, Color.red);
        
        // Check if the raycast hit something
        IsGrounded = ((hitGroundLeft.collider != null) || hitGroundRight.collider != null);

        //Same raycast position but now for the platforms
        RaycastHit2D hitPlatform =
            Physics2D.Raycast(castOrigin, Vector2.down, groundDistance, castFilterPlatform.layerMask);
        IsOnPlatform = hitPlatform.collider != null;
    }


    private void WallCast()
    {
        // Calculate the position at the side of the circle collider
        Vector2 castOrigin = _circleCollider2D.bounds.center;

        // The horizontal offset depends on the direction the character is facing
        float horizontalOffset = (_circleCollider2D.bounds.extents.x) * (wallCheckDirection.x > 0 ? 1 : -1);
        castOrigin.x += horizontalOffset;
        castOrigin += wallOffset;

        // Perform the raycast from the side of the circle collider
        RaycastHit2D hit = Physics2D.Raycast(castOrigin, wallCheckDirection, wallDistance, castFilterWall.layerMask);

        // Draw debug line for the cast
        Debug.DrawRay(castOrigin, wallCheckDirection * wallDistance, Color.red);

        // Check if the raycast hit something
        IsOnWall = hit.collider != null;
    }
}