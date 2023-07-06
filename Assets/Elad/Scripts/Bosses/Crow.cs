using System;
using System.Collections;
using System.Collections.Generic;
using BitStrap;
using Elad.Scripts;
using Elad.Scripts.Combat;
using UnityEngine;
using Logger = Nemesh.Logger;

public class Crow : MonoBehaviour
{
    [Header("Player Components")] private Damageable _damageablePlayer;
    [Header("Components")] private Animator _animator;

    private TrailRenderer _trailRenderer;

    private Transform target;
    private Vector3 sideTarget;
    [Header("Speed")] [SerializeField] private float regularSpeed;
    [SerializeField] private float attackingSpeed;
    [SerializeField] private float circleSpeedOne;
    [SerializeField] private float circleSpeedTwo;
    [SerializeField] private float attackingFromRoamingSpeed;

    [Header("Distance")] [SerializeField] private float startAttackDistance = 5f;

    [Header("Attack")] [SerializeField] private Vector2 knockBack = Vector2.right;
    [SerializeField] private float attackTime = 1f;
    private float attackTimer;
    [SerializeField] private float knockBackDelay = 0.1f;

    [Header("Circle Movement")] [SerializeField]
    private float afterAttackTime = 0.2f;

    private float _afterAttackTimer;
    [SerializeField] private Vector3 afterAttackOffset = Vector3.right;

    [SerializeField] private float thirdPositionYOffset = 4f;
    [SerializeField] private float secondPositionXOffset = 4f;
    private Vector3 afterAttackPositionOne;
    private Vector3 afterAttackPositionSecond;
    private Vector3 afterAttackPositionThird;


    enum CircleMovementStatus
    {
        First,
        Second,
        Three
    }

    private CircleMovementStatus _circleMovementStatus = CircleMovementStatus.First;

    private enum CrowModeEnum
    {
        MovingTowardPlayer,
        Attacking,
        AfterAttack,
        Roaming,
        AttackingFromRoaming
    }

    [SerializeField] private CrowModeEnum _crowMode = CrowModeEnum.MovingTowardPlayer;

    [Header("Rotation")] [SerializeField] private float rotationSpeed = 200f;
    private bool _sideFacingRight;

    [Header("Roaming Movement")] [SerializeField]
    private float roamingSpeed = 15f;

    private bool _roamingFirst;
    [SerializeField] private Transform roamingPositionFirst;
    [SerializeField] private Transform roamingPositionSecond;

    [Header("Boulder Throwing")] [SerializeField]
    private GameObject boulder;

    [SerializeField] private float xDistanceToThrow = 3f;
    [SerializeField] private Vector3 boulderInstantiateOffset = Vector3.down;
    private bool _canThrow = true;

    private void Awake()
    {
        _animator = GetComponentInChildren<Animator>();
        _trailRenderer = GetComponent<TrailRenderer>();
    }

    private void Start()
    {
        _damageablePlayer = PlayerStatus.PlayerDamageable;
        target = _damageablePlayer.gameObject.transform;
    }


    // Update is called once per frame
    void Update()
    {
        switch (_crowMode)
        {
            case CrowModeEnum.MovingTowardPlayer:
                sideTarget = target.position;
                MoveTowardPlayerRegular();
                SideHandler();
                AttackManager();
                break;

            case CrowModeEnum.Attacking:
                sideTarget = target.position;
                MoveTowardPlayerAttacking();
                AttackTimingHandler();
                break;

            case CrowModeEnum.AfterAttack:
                CircleMovement();
                break;

            case CrowModeEnum.Roaming:
                MoveRoaming();
                BoulderThrow();
                break;
        }

        SideHandler();
        RotateTowardsTarget();
    }

    private void BoulderThrow()
    {
        if (!_canThrow) return;
        
        float xDistance = Mathf.Abs(transform.position.x - target.position.x);
        if (xDistance < xDistanceToThrow)
        {
            Vector3 boulderInstantiatePosition = transform.position + boulderInstantiateOffset;
            Instantiate(boulder, boulderInstantiatePosition, Quaternion.identity);
            _animator.SetBool(AnimationStrings.withBoulder, false);
            _canThrow = false;
        }
    }


    private void AttackTimingHandler()
    {
        attackTimer -= Time.deltaTime;
        if (attackTimer <= 0)
        {
            _crowMode = CrowModeEnum.MovingTowardPlayer;
            StopAttack();
        }
    }


    private void MoveRoaming()
    {
        
        var roamingPosition = _roamingFirst ? roamingPositionFirst : roamingPositionSecond;
        // Move our position a step closer to the target.
        float step = roamingSpeed * Time.deltaTime; // calculate distance to move
        transform.position = Vector2.MoveTowards(transform.position, roamingPosition.position, step);

        float distance = Vector3.Distance(transform.position, roamingPosition.position);
        if (distance < 0.2f)
        {
            _roamingFirst = !_roamingFirst;
            sideTarget = _roamingFirst ? roamingPositionFirst.position : roamingPositionSecond.position;
            _animator.SetBool(AnimationStrings.withBoulder, true);
            _canThrow = true;
        }
    }
    
    

    private void MoveTowardPlayerAttackingFromRoaming()
    {
        // Move our position a step closer to the target.
        float step = attackingFromRoamingSpeed * Time.deltaTime; // calculate distance to move
        transform.position = Vector2.MoveTowards(transform.position, target.position, step);
    }
    
    private void MoveTowardPlayerRegular()
    {
        // Move our position a step closer to the target.
        float step = regularSpeed * Time.deltaTime; // calculate distance to move
        transform.position = Vector2.MoveTowards(transform.position, target.position, step);
    }

    private void MoveTowardPlayerAttacking()
    {
        // Move our position a step closer to the target.
        float step = attackingSpeed * Time.deltaTime; // calculate distance to move
        transform.position = Vector2.MoveTowards(transform.position, target.position, step);
    }

    private void SideHandler()
    {
        // Determine direction to the target
        Vector2 direction = sideTarget - transform.position;

        // Flip the sprite based on the direction to the target
        if (direction.x > 0)
        {
            transform.localScale = new Vector3(1, transform.localScale.y, transform.localScale.z);
            _sideFacingRight = true;
        }
        else if (direction.x < 0)
        {
            transform.localScale = new Vector3(-1, transform.localScale.y, transform.localScale.z);
            _sideFacingRight = false;
        }
    }

    private void AttackManager()
    {
        if (CanAttack())
        {
            StartAttack();
        }
    }

    private bool CanAttack()
    {
        float distance = Vector2.Distance(transform.position, target.position);
        return distance <= startAttackDistance;
    }

    [Button]
    private void StartAttack()
    {
        attackTimer = attackTime;
        _crowMode = CrowModeEnum.Attacking;
        _trailRenderer.emitting = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(TagStrings.playerTag))
        {
            DoAttack();
        }
    }

    private void DoAttack()
    {
        _crowMode = CrowModeEnum.AfterAttack;
        _animator.SetTrigger(AnimationStrings.crowAttack);
        _damageablePlayer.GotHit(1, knockBack, knockBackDelay);
        _afterAttackTimer = afterAttackTime;
        SetCircleParameters();
    }

    private void SetCircleParameters()
    {
        afterAttackPositionOne = target.position + afterAttackOffset;
        afterAttackPositionSecond = target.position -
                                    new Vector3(afterAttackOffset.x + secondPositionXOffset, -afterAttackOffset.y, 0f);
    }

    private void StopAttack()
    {
        _trailRenderer.emitting = false;
    }

    private void CircleMovement()
    {
        if (_afterAttackTimer > 0)
        {
            _afterAttackTimer -= Time.deltaTime;
            return;
        }

        Vector2 currentTarget = afterAttackPositionOne;
        switch (_circleMovementStatus)
        {
            case CircleMovementStatus.First:
                currentTarget = afterAttackPositionOne;
                break;

            case CircleMovementStatus.Second:
                currentTarget = afterAttackPositionSecond;
                break;

            case CircleMovementStatus.Three:
                currentTarget = afterAttackPositionThird;
                break;
        }

        var circleSpeed = _circleMovementStatus == CircleMovementStatus.First ? circleSpeedOne : circleSpeedTwo;
        // Move our position a step closer to the target.
        float step = circleSpeed * Time.deltaTime; // calculate distance to move
        transform.position = Vector2.MoveTowards(transform.position, currentTarget, step);

        SwitchCircleTarget(currentTarget);
    }

    private void SwitchCircleTarget(Vector2 currentTarget)
    {
        float distance = Vector2.Distance(transform.position, currentTarget);
        if (distance < 0.1)
        {
            switch (_circleMovementStatus)
            {
                case CircleMovementStatus.First:
                    _trailRenderer.emitting = false;
                    _circleMovementStatus = CircleMovementStatus.Second;
                    break;

                case CircleMovementStatus.Second:
                    _circleMovementStatus = CircleMovementStatus.Three;
                    afterAttackPositionThird = new Vector3(afterAttackPositionSecond.x,
                        target.position.y + thirdPositionYOffset, 0);
                    break;

                case CircleMovementStatus.Three:
                    _circleMovementStatus = CircleMovementStatus.First;
                    _crowMode = CrowModeEnum.MovingTowardPlayer;
                    break;
            }
        }

        sideTarget = currentTarget;
    }

    void RotateTowardsTarget()
    {
        // Determine direction to the target
        int mult = _sideFacingRight ? 1 : -1;
        Vector2 directionToTarget = (sideTarget - transform.position).normalized;
        directionToTarget *= mult;

        // Calculate the angle to the target
        float targetAngle = Mathf.Atan2(directionToTarget.y, directionToTarget.x) * Mathf.Rad2Deg;

        // Define maximum rotation angle
        float maxAngle = 40; // Replace with desired maximum angle

        // Check if the targetAngle is bigger than maxAngle
        if (Mathf.Abs(targetAngle) > maxAngle)
        {
            // Set newAngle directly to maxAngle (preserving the sign of targetAngle)
            transform.eulerAngles = new Vector3(0, 0, maxAngle * Mathf.Sign(targetAngle));
        }
        else
        {
            // Limit the rotation angle for a smoother rotation
            if (targetAngle > 180)
                targetAngle -= 360;

            // Rotate towards the target
            float rotationStep = rotationSpeed * Time.deltaTime;
            float newAngle = Mathf.MoveTowardsAngle(transform.eulerAngles.z, targetAngle, rotationStep);
            transform.eulerAngles = new Vector3(0, 0, newAngle);
        }
    }
}