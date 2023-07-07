using System;
using System.Collections;
using System.Collections.Generic;
using BitStrap;
using Elad.Scripts;
using Elad.Scripts.Combat;
using UnityEngine;
using Logger = Nemesh.Logger;
using Random = System.Random;

public class Crow : MonoBehaviour
{
    private Random _random;
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
    [SerializeField] private float roamingDownSpeed = 40f;
    [SerializeField] private float roamingWithBoulderSpeed = 10f;
    [SerializeField] private float roamingSpeed = 15f;

    [SerializeField] private float speedAddAmount = 0.5f;
    private float mainSpeed = 0;

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
        AttackingRegular,
        AfterAttack,
        Roaming,
        AttackingFromRoaming,
        AfterAttackingFromRoaming,
        GotHurt,
        Die,
        RoamingDown,
        RoamingUpFromDown
    }

    [SerializeField] private CrowModeEnum _crowMode = CrowModeEnum.MovingTowardPlayer;

    [Header("Rotation")] [SerializeField] private float rotationSpeed = 200f;
    private bool _sideFacingRight;

    [Header("Roaming Movement")] 

    [SerializeField] private Transform roamingPositionFirst;
    [SerializeField] private Transform roamingPositionSecond;
    private bool _roamingFirst;
    

    enum RoamingAttack
    {
        Boulder,
        Sprint
    }

    private RoamingAttack _roamingAttack = RoamingAttack.Boulder;

    [Header("Boulder Throwing")] [SerializeField]
    private GameObject boulder;

    [SerializeField] private float xDistanceToThrow = 3f;
    [SerializeField] private Vector3 boulderInstantiateOffset = Vector3.down;
    private bool _canThrow = true;
    private bool withBoulder;
    

    [Header("Sprint Attack")] [SerializeField]
    private float xDistanceToSprint = 6f;

    private bool _canSprintAttack = true;
    private bool _canSprint = true;
    [SerializeField] private float towardPlayerTime = 0.3f;
    private float _towardPlayerTimer;
    private Vector2 _sprintTarget;
    [SerializeField] private int xRandomMaxPosition = 1;
    [SerializeField] private int yAddPosition = 10;
    [SerializeField] private float afterAttackingFromRoamingDelayTime = 0.5f;
    private float _afterAttackingFromRoamingDelayTimer;

    [Header("Flesh")] private ColoredFlash _coloredFlash;
    [SerializeField] private Color beforeAttackColor = Color.white;
    [SerializeField] private Color hitColor = Color.red;
    private float _beforeAttackFleshTimer;
    [SerializeField] private float beforeAttackFleshTimeAdd = 0.1f;

    [Header("Got Hit")] [SerializeField] private int health = 6;
    [SerializeField] private float hitFleshTimeAdd = 0.1f;
    private float _hitAttackFleshTimer;
    private bool _gotHit;
    
    [Header("Animation")] [SerializeField] [Range(0,1)]private float animationSpeedMult = 0.5f;
    
    [Header("Tests")] [SerializeField] private bool justSprintAttack;
    [SerializeField] private float animationSpeed = 1;
    private void Awake()
    {
        _random = new Random();
        _animator = GetComponentInChildren<Animator>();
        _trailRenderer = GetComponent<TrailRenderer>();
        
    }

    private void Start()
    {
        _damageablePlayer = PlayerStatus.PlayerDamageable;
        target = _damageablePlayer.gameObject.transform;
        _coloredFlash = GetComponent<ColoredFlash>();
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

            case CrowModeEnum.AttackingRegular:
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
                StartSprintAttack();
                break;

            case CrowModeEnum.AttackingFromRoaming:
                MoveTowardPlayerAttackingFromRoaming();
                break;

            case CrowModeEnum.AfterAttackingFromRoaming:
                AfterAttackingFromRoaming();
                break;
            
            case CrowModeEnum.GotHurt:
                InHurt();
                break;
            
            case CrowModeEnum.Die:
                break;
            
            case CrowModeEnum.RoamingDown:
                MoveRoamingDown();
                break;
            
            case CrowModeEnum.RoamingUpFromDown:
                MoveUpFromDown();
                break;
        }

        SideHandler();
        RotateTowardsTarget();
        AnimationSpeedHandler();
        
    }

    private void ResetSpeed()
    {
        mainSpeed = 0;
    }
    private void AnimationSpeedHandler()
    {
        var s = mainSpeed * animationSpeedMult;
        _animator.speed = s;
    }
    private void AfterAttackingFromRoaming()
    {
        _afterAttackingFromRoamingDelayTimer -= Time.deltaTime;
        if (_afterAttackingFromRoamingDelayTimer < 0)
        {
            _crowMode = CrowModeEnum.Roaming;
            sideTarget = _roamingFirst ? roamingPositionFirst.position : roamingPositionSecond.position;
        }
    }

    [Button]
    public void DoFlashBeforeAttack()
    {
        _beforeAttackFleshTimer = _coloredFlash.Flash(beforeAttackColor) + beforeAttackFleshTimeAdd;
    }

    [Button]
    public void DoFlashHit()
    {
        _hitAttackFleshTimer = _coloredFlash.Flash(hitColor) + hitFleshTimeAdd;
        _coloredFlash.Flash(hitColor);
    }

    private void StartSprintAttack()
    {
        if (!_canSprint || _roamingAttack != RoamingAttack.Sprint) return;
        float xDistance = Mathf.Abs(transform.position.x - target.position.x);
        if (xDistance < xDistanceToSprint)
        {
            _crowMode = CrowModeEnum.AttackingFromRoaming;
            _towardPlayerTimer = towardPlayerTime;
            DoFlashBeforeAttack();
        }
    }


    private void BoulderThrow()
    {
        if (!_canThrow || _roamingAttack != RoamingAttack.Boulder) return;

        float xDistance = Mathf.Abs(transform.position.x - target.position.x);
        if (xDistance < xDistanceToThrow)
        {
            Vector3 boulderInstantiatePosition = transform.position + boulderInstantiateOffset;
            Instantiate(boulder, boulderInstantiatePosition, Quaternion.identity);
            withBoulder = false;
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


    private void ChoseRoamingAttack()
    {
        Array values = Enum.GetValues(typeof(RoamingAttack));
        _roamingAttack = (RoamingAttack) values.GetValue(_random.Next(values.Length));

        withBoulder = _roamingAttack == RoamingAttack.Boulder;
        
        _animator.SetBool(AnimationStrings.withBoulder, withBoulder);

        if (justSprintAttack)
        {
            _roamingAttack = RoamingAttack.Sprint;
        }
    }

    private void MoveRoamingDown()
    {
        var roamPos = _roamingFirst ? roamingPositionFirst.position : roamingPositionSecond.position;
        // Move our position a step closer to the target.
        
        float step = roamingSpeed * Time.deltaTime; // calculate distance to move
        roamPos = new Vector3(roamPos.x, transform.position.y,0);
        transform.position = Vector2.MoveTowards(transform.position, roamPos, step);

        float distance = Vector3.Distance(transform.position, roamPos);
        if (distance < 0.2f)
        {
            _crowMode = CrowModeEnum.RoamingUpFromDown;
        }
    }

    private void SetSpeed(float goalSpeed)
    {
        if (mainSpeed < goalSpeed)
        {
            mainSpeed += speedAddAmount;
        }

        else
        {
            mainSpeed = goalSpeed;
        }
    }
    
    private void MoveUpFromDown()
    {
        var roamingPosition = _roamingFirst ? roamingPositionFirst : roamingPositionSecond;
        // Move our position a step closer to the target.
        float step = roamingDownSpeed * Time.deltaTime; // calculate distance to move
        transform.position = Vector2.MoveTowards(transform.position, roamingPosition.position, step);

        float distance = Vector3.Distance(transform.position, roamingPosition.position);
        if (distance < 0.2f)
        {
            _crowMode = CrowModeEnum.Roaming;
        }
    }
    
    private void MoveRoaming()
    {
        var roamingPosition = _roamingFirst ? roamingPositionFirst : roamingPositionSecond;
        // Move our position a step closer to the target.

        float curSpeed = withBoulder ? roamingWithBoulderSpeed : roamingSpeed;
        animationSpeed = curSpeed;
        SetSpeed(curSpeed);
        float step = mainSpeed * Time.deltaTime; // calculate distance to move
        transform.position = Vector2.MoveTowards(transform.position, roamingPosition.position, step);

        float distance = Vector3.Distance(transform.position, roamingPosition.position);
        if (distance < 0.2f)
        {
            _roamingFirst = !_roamingFirst;
            sideTarget = _roamingFirst ? roamingPositionFirst.position : roamingPositionSecond.position;

            ChoseRoamingAttack();
            _canThrow = true;
            _canSprint = true;
            _canSprintAttack = true;
        }
    }


    [Button]
    public void AttackFromRoaming()
    {
        _crowMode = CrowModeEnum.AttackingFromRoaming;
        _towardPlayerTimer = towardPlayerTime;
    }

    private void MoveTowardPlayerAttackingFromRoaming()
    {
        _beforeAttackFleshTimer -= Time.deltaTime;
        if (_beforeAttackFleshTimer >= 0) return;

        _towardPlayerTimer -= Time.deltaTime;

        if (_towardPlayerTimer >= 0)
        {
            var r = _random.Next(-xRandomMaxPosition, xRandomMaxPosition);
            _sprintTarget = target.position + new Vector3(r, -yAddPosition, 0);
        }

        sideTarget = _sprintTarget;
        // Move our position a step closer to the target.
        animationSpeed = attackingFromRoamingSpeed;
        
        float step = attackingFromRoamingSpeed * Time.deltaTime; // calculate distance to move
        transform.position = Vector2.MoveTowards(transform.position, _sprintTarget, step);

        float distance = Vector3.Distance(transform.position, _sprintTarget);
        if (distance < 0.2f)
        {
            
            _crowMode = CrowModeEnum.RoamingDown;
            _canSprint = false;
            sideTarget = _roamingFirst ? roamingPositionFirst.position : roamingPositionSecond.position;
        }
    }

    
    
    private void MoveTowardPlayerRegular()
    {
        // Move our position a step closer to the target.
        animationSpeed = regularSpeed;
        SetSpeed(regularSpeed);
        float step = mainSpeed * Time.deltaTime; // calculate distance to move
        transform.position = Vector2.MoveTowards(transform.position, target.position, step);
    }

    private void MoveTowardPlayerAttacking()
    {
        // Move our position a step closer to the target.
        animationSpeed = attackingSpeed;
        SetSpeed(attackingSpeed);
        float step = mainSpeed * Time.deltaTime; // calculate distance to move
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
        _crowMode = CrowModeEnum.AttackingRegular;
        _trailRenderer.emitting = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(TagStrings.playerTag))
        {
            DoAttack();
        }

        else if (other.CompareTag(TagStrings.spikesTag))
        {
            GotHitStart();
        }
    }

    private void GotHitStart()
    {
        
        
        health -= 1;
        if (health == 0)
        {
            DieStart();    
        }

        _animator.SetTrigger(AnimationStrings.hurt);
        _canSprint = false;
        _crowMode = CrowModeEnum.GotHurt;
        _gotHit = true;
        DoFlashHit();
        sideTarget = target.position;
    }

    private void InHurt()
    {
        _hitAttackFleshTimer -= Time.deltaTime;
        if (_hitAttackFleshTimer < 0)
        {
            _crowMode = CrowModeEnum.Roaming;
            sideTarget = _roamingFirst ? roamingPositionFirst.position : roamingPositionSecond.position;
        }
    }
    
    private void DieStart()
    {
        _crowMode = CrowModeEnum.Die;
    }

    private void DoAttack()
    {
        if (_crowMode == CrowModeEnum.AttackingRegular)
        {
            _crowMode = CrowModeEnum.AfterAttack;
            _afterAttackTimer = afterAttackTime;
            SetCircleParameters();
        }
        else
        {
            if (!_canSprintAttack) return;
            _crowMode = CrowModeEnum.AfterAttackingFromRoaming;
            _canSprintAttack = false;
            _canSprint = false;
            _afterAttackingFromRoamingDelayTimer = afterAttackingFromRoamingDelayTime;
            sideTarget = target.position;
        }

        _animator.SetTrigger(AnimationStrings.crowAttack);
        _damageablePlayer.GotHit(1, knockBack, knockBackDelay);
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
        animationSpeed = circleSpeed;
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