using System;
using System.Collections;
using System.Collections.Generic;
using BitStrap;
using Elad.Events;
using Elad.Scripts;
using Elad.Scripts.Combat;
using FMOD.Studio;
using Managers;
using Mechanics.Enemies;
using Mechanics.UI.Menus;
using UnityEngine;
using Logger = Nemesh.Logger;
using Random = System.Random;
using static Mechanics.Enemies.CorotuineUtils;

public class Crow : MonoBehaviour, ICanBeAttacked
{
    private Random _random;

    [Header("Y Position While Following ")] [SerializeField]
    private float yMaxDistanceWhileFollow = 1f;

    [Header("Trail renderer")] private bool useTrailRenderer;

    [Header("Player Components")] private Damageable _damageablePlayer;
    [Header("Components")] private Animator _animator;

    private TrailRenderer _trailRenderer;
    private ParticleSystem _pS;
    private SpriteRenderer _sp;

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
    [SerializeField] private float maxXDistanceWhileFollowing = 20;

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

    private enum BossLevelState
    {
        Following,
        Roaming
    }

    [SerializeField] private BossLevelState bossLevelState = BossLevelState.Following;

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
        RoamingUpFromDown,
        None,
        NeedToStartRoaming
    }

    [SerializeField] private CrowModeEnum _crowMode = CrowModeEnum.MovingTowardPlayer;

    [Header("Rotation")] [SerializeField] private float rotationSpeed = 200f;
    private bool _sideFacingRight;

    [Header("Roaming Movement")] [SerializeField]
    private float switchToRoamingTimer = 2f;

    [SerializeField] private Transform roamingRight;

    [SerializeField] private Transform roamingLeft;
    private bool _roamingFirst;


    enum RoamingAttack
    {
        Boulder,
        Sprint
    }

    private RoamingAttack _roamingAttack = RoamingAttack.Boulder;
    [SerializeField] private float yRoamingDifference = 2f;

    [Header("Boulder Throwing")] [SerializeField]
    private GameObject boulder;

    [SerializeField] private float xDistanceToThrow = 3f;
    [SerializeField] private Vector3 boulderInstantiateOffset = Vector3.down;
    private bool _canThrow = true;
    private bool withBoulder;


    [Header("Sprint Attack")] [SerializeField]
    private float addToXSprintAttack = 1f;

    [SerializeField] private float xDistanceToSprint = 6f;

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
    private bool _canGetHit = true;

    [Header("Animation")] [SerializeField] [Range(0, 1)]
    private float animationSpeedMult = 0.5f;

    [Header("Death")] [SerializeField] private float delayEndOpenMenu = 5f;
    private bool _isDead;
    [Header("Tests")] [SerializeField] private bool justSprintAttack;
    [SerializeField] private float animationSpeed = 1;
    [SerializeField] private bool _inTest;

    [Header("Sounds")] private EventInstance _flySound;

    [Header("Return Positions")] private Vector3 _initiationPosition;
    [SerializeField] private Transform _startRoamingPosition;

    private void OnEnable()
    {
        BossEvents.StartRoaming.AddListener(StartToSwitchToRoaming);
        BossEvents.StopBossMovement.AddListener(StopMovement);
        BossEvents.BossStart.AddListener(BossStart);
        characterEvents.PlayerDied.AddListener(PlayerDied);
    }

    private void OnDisable()
    {
        BossEvents.StartRoaming.RemoveListener(StartToSwitchToRoaming);
        BossEvents.StopBossMovement.RemoveListener(StopMovement);
        BossEvents.BossStart.RemoveListener(BossStart);
        characterEvents.PlayerDied.RemoveListener(PlayerDied);
    }


    private void Awake()
    {
        _random = new Random();
        _animator = GetComponentInChildren<Animator>();
        _trailRenderer = GetComponent<TrailRenderer>();
        _pS = GetComponentInChildren<ParticleSystem>();
        _sp = GetComponentInChildren<SpriteRenderer>();
        BossSleep();
        _initiationPosition = transform.position;
    }

    private void Start()
    {
        _damageablePlayer = PlayerStatus.PlayerDamageable;
        target = _damageablePlayer.gameObject.transform;
        _coloredFlash = GetComponent<ColoredFlash>();
        _flySound = AudioManager.instance.CreatEventInstance(FMODEvents.instance.crowFly);
    }

    private CrowModeEnum lastCrowMode = CrowModeEnum.None;

    // Update is called once per frame
    void Update()
    {
        if (GeneralGameManager.IsGamePause) return;
        if (lastCrowMode != _crowMode)
        {
            lastCrowMode = _crowMode;
        }

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

            case CrowModeEnum.None:
                return;

            case CrowModeEnum.NeedToStartRoaming:
                SwitchToRoaming();
                break;
        }


        SideHandler();
        RotateTowardsTarget();
        AnimationSpeedHandler();
        UpdateFlySound();
    }

    private void setAlf(int alf)
    {
        // _crowMode = CrowModeEnum.MovingTowardPlayer;
        Color col = _sp.color;
        col.a = alf;
        _sp.color = col;
        if (alf == 1)
            _pS.Play();
        else
            _pS.Stop();
    }

    private void BossSleep()
    {
        setAlf(0);
        _pS.Stop();
        _crowMode = CrowModeEnum.None;
    }

    private void BossStart()
    {
        setAlf(1);
        _pS.Play();
        _crowMode = CrowModeEnum.MovingTowardPlayer;
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
        mainSpeed = 0;

        _afterAttackingFromRoamingDelayTimer -= Time.deltaTime;
        if (_afterAttackingFromRoamingDelayTimer < 0)
        {
            _pS.Play();
            _crowMode = CrowModeEnum.Roaming;
            sideTarget = _roamingFirst ? roamingRight.position : roamingLeft.position;
        }
    }

    [Button]
    public void DoFlashBeforeAttack()
    {
        _beforeAttackFleshTimer = _coloredFlash.Flash(beforeAttackColor) + beforeAttackFleshTimeAdd;
        AudioManager.instance.PlayOneShot(FMODEvents.instance.startAttack, transform.position);
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
            _animator.SetBool(AnimationStrings.crowAttackFromRoaming, true);
            _crowMode = CrowModeEnum.AttackingFromRoaming;
            TrailRenderHandler(true);
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
            AudioManager.instance.PlayOneShot(FMODEvents.instance.rockThrow, transform.position);

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
            _pS.Play();
            _crowMode = CrowModeEnum.MovingTowardPlayer;
            StopAttack();
        }
    }


    private bool isFirst = true;

    private void ChoseRoamingAttack()
    {
        Array values = Enum.GetValues(typeof(RoamingAttack));
        _roamingAttack = (RoamingAttack) values.GetValue(_random.Next(values.Length));

        if (isFirst)
        {
            isFirst = false;
            _roamingAttack = RoamingAttack.Boulder;
        }


        withBoulder = _roamingAttack == RoamingAttack.Boulder;

        _animator.SetBool(AnimationStrings.withBoulder, withBoulder);

        if (justSprintAttack)
        {
            _roamingAttack = RoamingAttack.Sprint;
        }
    }

    private void MoveRoamingDown()
    {
        var roamPos = _roamingFirst ? roamingRight.position : roamingLeft.position;
        // Move our position a step closer to the target.
        mainSpeed = roamingDownSpeed;
        float step = roamingDownSpeed * Time.deltaTime; // calculate distance to move
        roamPos = new Vector3(roamPos.x, transform.position.y, 0);
        transform.position = Vector2.MoveTowards(transform.position, roamPos, step);

        float distance = Vector3.Distance(transform.position, roamPos);
        if (distance < 0.2f)
        {
            _crowMode = CrowModeEnum.RoamingUpFromDown;
            _animator.SetBool(AnimationStrings.crowAttackFromRoaming, false);
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

    private void SwitchToRoaming()
    {
        switchToRoamingTimer -= Time.deltaTime;
        if (switchToRoamingTimer < 0)
        {
            if (_inTest)
            {
                BossStart();
            }

            bossLevelState = BossLevelState.Roaming;
            TrailRenderHandler(false);
            _crowMode = CrowModeEnum.Roaming;
            // _startRoamingPosition.position = transform.position;
            _roamingFirst = false;
            sideTarget = roamingLeft.position;
        }
    }

    private void StopMovement()
    {
        _crowMode = CrowModeEnum.None;
    }

    private void StartToSwitchToRoaming()
    {
        _crowMode = CrowModeEnum.NeedToStartRoaming;
    }


    private void MoveUpFromDown()
    {
        var roamingPosition = _roamingFirst ? roamingRight : roamingLeft;
        // Move our position a step closer to the target.
        mainSpeed = roamingDownSpeed;
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
        var roamingPosition = _roamingFirst ? roamingRight : roamingLeft;
        // Move our position a step closer to the target.

        float curSpeed = withBoulder ? roamingWithBoulderSpeed : roamingSpeed;
        animationSpeed = curSpeed;
        SetSpeed(curSpeed);
        float step = mainSpeed * Time.deltaTime; // calculate distance to move
        transform.position = Vector2.MoveTowards(transform.position, roamingPosition.position, step);

        float distance = Vector3.Distance(transform.position, roamingPosition.position);
        if (distance < 0.2f)
        {
            if (_isDead)
            {
                BossEvents.BossDead.Invoke();
                MenuManager.Menu.StartCoroutine(DelayExecution(delayEndOpenMenu,
                    () =>
                    {
                        MenuManager.Menu.OpenEndLevelMenu();
                    })
                );
                Destroy(gameObject, 2.5f);
            }

            _roamingFirst = !_roamingFirst;
            sideTarget = _roamingFirst ? roamingRight.position : roamingLeft.position;

            ChoseRoamingAttack();
            _canThrow = true;
            _canSprint = true;
            _canSprintAttack = true;
            _canGetHit = true;
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
            bool playerRight = (transform.position.x - target.position.x) < 0;
            var r = addToXSprintAttack;
            if (!playerRight)
            {
                r *= -1;
            }

            // var r = _random.Next(-xRandomMaxPosition, xRandomMaxPosition);
            _sprintTarget = target.position + new Vector3(r, -yAddPosition, 0);
        }

        sideTarget = _sprintTarget;
        // Move our position a step closer to the target.
        animationSpeed = attackingFromRoamingSpeed;
        mainSpeed = attackingFromRoamingSpeed;
        float step = attackingFromRoamingSpeed * Time.deltaTime; // calculate distance to move
        transform.position = Vector2.MoveTowards(transform.position, _sprintTarget, step);

        float distance = Vector3.Distance(transform.position, _sprintTarget);
        if (distance < 0.2f)
        {
            _crowMode = CrowModeEnum.RoamingDown;
            _canSprint = false;
            TrailRenderHandler(false);
            sideTarget = _roamingFirst ? roamingRight.position : roamingLeft.position;
        }
    }


    private void MoveTowardPlayerRegular()
    {
        // Move our position a step closer to the target.
        animationSpeed = regularSpeed;
        SetSpeed(regularSpeed);

        InRangeFollowing();

        float step = mainSpeed * Time.deltaTime; // calculate distance to move
        transform.position = Vector2.MoveTowards(transform.position, target.position, step);
    }

    private void InRangeFollowing()
    {
        float distanceX = transform.position.x - target.position.x;
        if (Mathf.Abs(distanceX) > maxXDistanceWhileFollowing)
        {
            float newX = distanceX > 0
                ? transform.position.x - (distanceX - maxXDistanceWhileFollowing)
                : transform.position.x + (Mathf.Abs(distanceX) - maxXDistanceWhileFollowing);
            transform.position = new Vector3(newX, transform.position.y, 0);
        }

        float distanceY = transform.position.y - target.position.y;
        if (Mathf.Abs(distanceY) > yMaxDistanceWhileFollow)
        {
            float targetY = distanceY > 0
                ? transform.position.y - (distanceY - yMaxDistanceWhileFollow)
                : transform.position.y + (Mathf.Abs(distanceY) - yMaxDistanceWhileFollow);

            // Modify the interpolation speed to your liking
            float smoothSpeed = 0.05f;
            float newY = Mathf.SmoothStep(transform.position.y, targetY, smoothSpeed);

            transform.position = new Vector3(transform.position.x, newY, 0);
        }
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
        TrailRenderHandler(true);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        _animator.SetBool(AnimationStrings.crowAttackFromRoaming, false);

        if (_crowMode == CrowModeEnum.None) return;
        if ((_crowMode == CrowModeEnum.AttackingFromRoaming || _crowMode == CrowModeEnum.AttackingRegular) &&
            other.CompareTag(TagStrings.playerTag))
        {
            AudioManager.instance.PlayOneShot(FMODEvents.instance.crowYellShort, transform.position);
            // TrailRenderHandler(false);
            // _pS.Stop();
            _canGetHit = false;
            _canSprint = false;
            DoAttack();
        }
    }

    public bool Hurt(AttackParameters attackParameters)
    {
        if (!CanGetHit())
        {
            return false;
        }

        Logger.Log($"Hurt by: {attackParameters.Attacker}");

        AudioManager.instance.PlayOneShot(FMODEvents.instance.crowGetHurt, transform.position);
        _canGetHit = false;
        _pS.Stop();
        GotHitStart();
        TrailRenderHandler(false);
        return true;
    }

    public Transform GetTransform()
    {
        return transform;
    }

    private bool CanGetHit()
    {
        return _crowMode == CrowModeEnum.AttackingFromRoaming && _canGetHit;
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
            sideTarget = _roamingFirst ? roamingRight.position : roamingLeft.position;
        }
    }

    private void DieStart()
    {
        _isDead = true;
        AudioManager.instance.SetBossMusic(4);
    }

    private void DoAttack()
    {
        if (bossLevelState == BossLevelState.Following)
        {
            _crowMode = CrowModeEnum.AfterAttack;
            _afterAttackTimer = afterAttackTime;
            SetCircleParameters();
        }
        else if (bossLevelState == BossLevelState.Roaming)
        {
            {
                if (!_canSprintAttack) return;
                // _crowMode = CrowModeEnum.AfterAttackingFromRoaming;
                _canSprintAttack = false;
                _canSprint = false;
                _afterAttackingFromRoamingDelayTimer = afterAttackingFromRoamingDelayTime;
                // sideTarget = target.position;
            }
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
        TrailRenderHandler(false);
    }

    private void CircleMovement()
    {
        if (_afterAttackTimer > 0)
        {
            _afterAttackTimer -= Time.deltaTime;
            return;
        }


        _pS.Play();
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
                    TrailRenderHandler(false);
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
        if (bossLevelState == BossLevelState.Following) return;
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

    private void UpdateFlySound()
    {
        float distance = Vector3.Distance(transform.position, PlayerStatus.Player.transform.position);
        _flySound.setParameterByName(MusicStrings.FlyVolume, distance);
        _flySound.setParameterByName(MusicStrings.FlyPitch, mainSpeed);

        PLAYBACK_STATE playbackState;
        _flySound.getPlaybackState(out playbackState);
        if (playbackState.Equals(PLAYBACK_STATE.STOPPED))
        {
            _flySound.start();
        }
    }

    private void TrailRenderHandler(bool value)
    {
        if (useTrailRenderer)
            _trailRenderer.emitting = value;
    }

    private void PlayerDied()
    {
        _crowMode = CrowModeEnum.None;
        StopMovement();
        switch (bossLevelState)
        {
            case BossLevelState.Following:
                transform.position = _initiationPosition;
                Logger.Log("initial pos");
                break;

            case BossLevelState.Roaming:
                Logger.Log("roaming pos");
                _roamingFirst = false;
                transform.position = _startRoamingPosition.position;
                break;
        }

        setAlf(0);
    }
}