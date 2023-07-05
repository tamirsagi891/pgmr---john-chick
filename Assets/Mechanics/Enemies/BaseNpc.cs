using System.Collections.Generic;
using Avrahamy;
using BitStrap;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.Serialization;
using static Mechanics.Enemies.CorotuineUtils;
using Logger = Nemesh.Logger;

namespace Mechanics.Enemies
{
    [AddComponentMenu("NPC/Base Npc", -1)]
    [SelectionBase]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(StatsHandler))]
    // TODO: create new class that just requires the Components in the right order
    [RequireComponent(typeof(Rigidbody2D))]
    public class BaseNpc : MonoBehaviour, IAttacker, ICanBeAttacked
    {

        #region Inspector

        [Header("Base NPC Fields")]
        [ContextMenuItem("Log Stats", "TestContextMenu")]
        [SerializeField]
        [Tooltip("The data this NPC will reference")]
        [InlineScriptableObject]
        protected NpcDataScriptable myData;

        [SerializeField]
        [RequiredReference]
        protected NpcAnimationControls animationControls;

        [SerializeField]
        [RequiredReference]
        protected AttackBehaviour attackController;

        [SerializeField]
        protected bool canDetectPlayer = true;

        [FormerlySerializedAs("attackTime")]
        [Space]
        [HelpBox("THIS SHOULD BE FROM ANIMATION NOT HERE! But im too lazy.",
            HelpBoxAttribute.MessageType.Warning)]
        [SerializeField]
        protected float stopMovementDuringAttackTime = 1f;

        [SerializeField]
        protected float attackStartAfterTime = 1f;

        [SerializeField]
        protected float hurtTime = 1f;
        
        [SerializeField]
        private PassiveTimer hurtInvTime = new(1);

        [SerializeField]
        protected PassiveTimer dashTime = new(0.2f);

        [SerializeField]
        protected PassiveTimer stopAtDashEndTimer = new(0f);

        [SerializeField]
        private bool tryDashBeforeJump;

        [SerializeField]
        [Tooltip("After how many second of not moving try to switch direction")]
        protected bool useNotMovingTimer = true;

        [SerializeField]
        [Tooltip("After how many second of not moving try to switch direction")]
        protected PassiveTimer notMovingTimer = new(0.25f);

        [Space]
        [SerializeField]
        protected bool stopMovementWhileTargetExists = true;

        [SerializeField]
        protected bool detectEdges = true;

        [SerializeField]
        protected bool canAirAttack; // TODO: move to getter and check Ground/Flying enemy, or to inherited class.

        [SerializeField]
        protected float minDistanceForMovementWhenHavePlayer = 1f;

        [Space]
        [SerializeField]
        protected bool removeAfterDeath;

        [SerializeField]
        protected PassiveTimer removeAfterDeathTimer = new(10f);

        [Space]
        [SerializeField]
        public NpcEvents events;
        
        [SerializeField]
        protected Transform offsetPosition;

        [Space]
        [Header("Debug")]
        [SerializeField]
        protected bool debug;

        #endregion

        #region Public Properties

        public bool CanDetectPlayer
        {
            get => canDetectPlayer;
            set => canDetectPlayer = value;
        }

        public INpcMovementBehaviour MovementBehaviour
        {
            get => _movementBehaviour;
            set
            {
                if (_movementBehaviour != null)
                {
                    // TODO: handle case of multiple - swap for now, but maybe multiple in the future?
                    // Logger.Log("Multiple behaviours!");
                }

                _movementBehaviour = value;
            }
        }

        public virtual bool IsGrounded
        {
            get => animationControls.IsGrounded;
            set => animationControls.IsGrounded = value;
        }

        public virtual ICanBeAttacked PlayerContact
        {
            get => _playerContact;
            set
            {
                _playerContact = value;
                if (IsDead)
                {
                    return;
                }

                // TODO: add script for attack strategy
                HasPlayerContact = value != null;
                if (!CanDetectPlayer)
                {
                    return;
                }

                if (HasPlayerContact)
                {
                    WalkTarget = value!.GetTransform();
                    events.onPlayerDetected.Invoke();
                    if (MovementBehaviour != null) // TODO: remove this on build
                    {
                        MovementBehaviour.EnabledBehaviour = false;
                    }
                }
                else
                {
                    if (MovementBehaviour != null) // TODO: remove this on build
                    {
                        MovementBehaviour.EnabledBehaviour = true;
                        MovementBehaviour.GoToNextPoint();
                    }
                }

                if (debug)
                {
                    Logger.Log($"{NpcDataScriptable.npcName} Contact With Player: {_playerContact}", this);
                }
            }
        }

        protected bool HasPlayerContact { get; set; }

        public NpcDataScriptable NpcDataScriptable
        {
            get => myData;
            set => myData = value;
        }

        public List<ICanBeAttacked> AttackTargets { get; set; } = new();

        public virtual Transform WalkTarget
        {
            get => _walkTarget;
            set
            {
                value = WalkTargetHelper(value);
                if (debug)
                {
                    Logger.Log($"Target old: {_walkTarget} | new: {value}", this);
                }

                _walkTarget = value;
                HasDestination = _walkTarget != null;
            }
        }

        protected virtual Transform WalkTargetHelper(Transform value)
        {
            if (HasPlayerContact && !CanDetectPlayer && value == PlayerContact.GetTransform())
            {
                if (animationControls.CanMove && MovementBehaviour != null) // TODO: remove this on build
                {
                    MovementBehaviour.EnabledBehaviour = true;
                    MovementBehaviour.GoToNextPoint();
                }
            }

            return value;
        }

        public bool EdgeInFront
        {
            get => edgeInFront;
            set => edgeInFront = value;
            // checkEdge = edgeInFront;
        }

        public bool DetectEdges
        {
            get => detectEdges;
            set => detectEdges = value;
        }

        public Direction CurrentDirection => animationControls.Direction;
        public bool IsDashing => dashTime.IsSet && dashTime.IsActive;

        public bool CanAttack { get; protected set; } = true;

        public bool OnCd => AttackCdTimer.IsSet && AttackCdTimer.IsActive;

        // public NpcStats Stats => MyStatsHandler.CurrentStats; 
        public float Cooldown => myStatsHandler.Cooldown;

        public StatsHandler MyStatsHandler => myStatsHandler;

        public Transform OffsetPosition => offsetPosition;

        #endregion

        #region Private Fields

        private ICanBeAttacked _playerContact; // TODO: change to any target?

        protected StatsHandler myStatsHandler;
        private INpcMovementBehaviour _movementBehaviour;
        private Transform _walkTarget; // TODO: create type of WalkTarget?
        protected bool HasDestination;

        protected Rigidbody2D MyRigidbody;
        protected Vector2 DesiredVelocity;
        protected Vector2 Velocity;

        protected float DashAlertDistance => HasDashControl ? DashAlertControl.Radius * transform.lossyScale.x : 7f;
        protected DashAndAlertControl DashAlertControl;
        protected bool HasDashControl;

        protected readonly PassiveTimer AttackCdTimer = new();
        protected readonly PassiveTimer StopMovementTimer = new();

        protected bool ShouldMove = true;
        protected bool IsDead;

        protected Direction DefaultDirection;
        protected bool HasDefaultDirection;

        protected bool edgeInFront;

        // protected bool checkEdge;

        #endregion

        #region MonoBehaviour

        protected virtual void Awake()
        {
            myStatsHandler = GetComponent<StatsHandler>();
            MyRigidbody = GetComponent<Rigidbody2D>();
            notMovingTimer.Clear();
            DashAlertControl = GetComponentInChildren<DashAndAlertControl>();
            HasDashControl = DashAlertControl != null;
        }

        protected void OnDisable()
        {
            if (debug)
            {
                Logger.Log($"{NpcDataScriptable.npcName} Removed", Color.yellow, this);
            }

            events.onDisable.Invoke(this);
        }

        protected virtual void Update()
        {
            if (HandleTimersAndCheckIfNeedsUpdate())
            {
                return;
            }

            HandleAttackUpdate();
        }

        protected bool HandleTimersAndCheckIfNeedsUpdate()
        {
            if (IsDead)
            {
                if (!removeAfterDeath || removeAfterDeathTimer.IsActive)
                {
                    return true;
                }

                gameObject.SetActive(false);
            }

            if (dashTime.IsSet && !dashTime.IsActive)
            {
                StopDash();
            }

            if (!IsDead && StopMovementTimer.IsSet && !StopMovementTimer.IsActive &&
                (!stopMovementWhileTargetExists || AttackTargets.Count <= 0))
            {
                StartMovement();
            }

            return false;
        }

        public virtual void StopDash()
        {
            if (!IsDashing)
            {
                return;
            }

            dashTime.Clear();
            myStatsHandler.CurrentStats.movementSpeed -= myStatsHandler.CurrentStats.extraDashSpeed;
            animationControls.StopDirectionSwitch = false;
            animationControls.IsDashing = false;
            events.onDashEnd.Invoke();
            StopMovement(stopAtDashEndTimer.Duration);
        }

        protected virtual void HandleAttackUpdate()
        {
            if (!CanAttack || AttackTargets.Count == 0 || !canAirAttack && !IsGrounded)
            {
                return;
            }

            if (AttackCdTimer.IsSet && AttackCdTimer.IsActive) // TODO: this repeats - make it a method of PassiveTImer
            {
                return;
            }

            Attack();
        }

        protected void FixedUpdate()
        {
            animationControls.IsMoving = MyRigidbody.velocity.sqrMagnitude > 0.05f;

            if (HasDestination)
            {
                HandleMovementFixedUpdate();
            }
            else
            {
                ShouldMove = false;
            }
        }

        protected virtual void HandleMovementFixedUpdate()
        {
            var shouldSwitch = ShouldSwitchDirectionWhenNotMoving();

            if (animationControls.CanMove)
            {
                if (IsGrounded) // TODO: Dash + Jump doesnt work cause of this
                {
                    var targetLeft = WalkTarget.position.x < transform.position.x;
                    animationControls.Direction = targetLeft ? Direction.Left : Direction.Right;

                    var speed = myStatsHandler.CurrentStats.movementSpeed;
                    speed = animationControls.Direction == Direction.Left ? -speed : speed;
                    DesiredVelocity.x = speed;

                    ShouldMove = Mathf.Abs(DesiredVelocity.x) > 0.01f;
                    // if (HasPlayerContact)
                    // {
                    //     animationControls.StopDirectionSwitch = Mathf.Abs(WalkTarget.position.x - transform.position.x) < minDistanceForMovementWhenHavePlayer;
                    // }

                    if ((DetectEdges && EdgeInFront) || shouldSwitch) // TODO: ignore edges in pursuit?
                    {
                        HandleDirectionSwitch();
                    }
                }

                RunWithoutAcceleration();
            }
            else if (HasDefaultDirection)
            {
                animationControls.Direction = DefaultDirection;
            }
        }

        protected virtual void AttackAllTarget()
        {
            events.onAttack.Invoke();
            var attackParameters = GetAttackParameters();

            foreach (var target in AttackTargets)
            {
                AttackTargetUsingParams(target, attackParameters);
            }
        }

        #endregion


        #region Public Methods

        public void HandleDirectionSwitch()
        {
            EdgeInFront = false;
            StopDash();
            DesiredVelocity.x = 0f;
            if (animationControls.StopDirectionSwitch)
            {
                return; // TODO: move the check inside animationControls.SwitchDirection
            }

            if (MovementBehaviour is {EnabledBehaviour: true})
            {
                MovementBehaviour?.GoToNextPoint();
            }

            animationControls.SwitchDirection();
        }

        [Button]
        public void Jump()
        {
            if (!IsGrounded || !animationControls.CanMove)
            {
                return;
            }

            if (tryDashBeforeJump)
            {
                if (dashTime.IsSet && dashTime.IsActive)
                {
                    return;
                }

                Dash();
            }

            MyRigidbody.AddForce(new Vector2(0f, myStatsHandler.CurrentStats.jumpForce), ForceMode2D.Impulse);
            animationControls.Jump = true;
            events.onJump.Invoke();
        }

        [Button]
        public virtual void Dash()
        {
            if (!animationControls.CanMove)
            {
                return; // TODO: dash when not moving should still dash.
            }

            if (dashTime.IsSet && dashTime.IsActive)
            {
                return;
            }

            dashTime.Start();
            animationControls.StopDirectionSwitch = true;
            myStatsHandler.CurrentStats.movementSpeed += myStatsHandler.CurrentStats.extraDashSpeed;
            DesiredVelocity.x = Mathf.Sign(DesiredVelocity.x) * myStatsHandler.CurrentStats.movementSpeed;
            // TODO: Copy elad's implementation
            animationControls.IsDashing = true;
            events.onDash.Invoke();
        }

        public bool Attack(ICanBeAttacked attackTarget)
        {
            return Attack(attackTarget, true);
        }
        public bool Attack(ICanBeAttacked attackTarget, bool stopMovement)
        {
            // TODO: if can attack, valid start, etc...
            if (AttackCdTimer.IsSet && AttackCdTimer.IsActive)
            {
                return false;
            }

            HandleAttackStart(stopMovement);
            AttackCdTimer.Start(myStatsHandler.CurrentStats.cooldown);

            var attackParameters = GetAttackParameters();

            StartCoroutine(DelayExecution(attackStartAfterTime,
                    () => { AttackTargetUsingParams(attackTarget, attackParameters); }
                )
            );

            return true;
        }

        public Transform GetTransform() => transform;

        protected virtual void AttackTargetUsingParams(ICanBeAttacked attackTarget, AttackParameters attackParameters)
        {
            var succeeded = attackTarget.Hurt(attackParameters);
            events.onAttack.Invoke();
        }

        public virtual AttackParameters GetAttackParameters()
        {
            var knockBack = myStatsHandler.CurrentStats.knockBack;
            if (CurrentDirection != Direction.Right)
            {
                knockBack = new Vector2(-knockBack.x, knockBack.y);
            }

            return new AttackParameters(
                attacker: this,
                damage: GetDamage(),
                knockBack: knockBack,
                type: myStatsHandler.CurrentStats.type,
                followTransform: transform,
                shotSpeed: myStatsHandler.CurrentStats.shotSpeed,
                direction: CurrentDirection,
                knockBackDelay: myStatsHandler.CurrentStats.knockBackDelay);
        }

        [Button]
        public void Attack()
        {
            HandleAttackStart(true);

            AttackCdTimer.Start(myStatsHandler.CurrentStats.cooldown);

            StartCoroutine(DelayExecution(attackStartAfterTime, AttackAllTarget));
        }

        public void HandleAttackStart(bool stopMovement)
        {
            animationControls.Attack = true;
            events.onAttackStart.Invoke();
            if (stopMovement)
            {
                StopMovement(stopMovementDuringAttackTime);
            }
        }


        public bool Hurt(AttackParameters attackParameters)
        {
            if (IsDead)
            {
                return false;
            }

            if (attackParameters.Type != AttackType.Regular)
            {
                Logger.Log("No behaviour for non-regular attack", Color.yellow, this);
            }

            var dmgTaken = attackParameters.Damage;
            var knockBack = attackParameters.KnockBack;
            
            if (dmgTaken <= 0)
            {
                return false;
            }

            var ret = TakeDamage(dmgTaken);
            if (debug && ret)
            {
                Logger.Log($"Attacked by {attackParameters.Attacker} for {dmgTaken}");
            }

            return ret;
        }

        [Button]
        public void Death()
        {
            IsDead = true;
            animationControls.IsDead = true;

            StopMovementHelper();
            MyRigidbody.gravityScale = 1f;
            MyRigidbody.constraints |= RigidbodyConstraints2D.FreezePositionX; // TODO: do we want to?

            events.onDeath.Invoke();
            // TODO: collider to sprite?

            if (debug)
            {
                Logger.Log($"{NpcDataScriptable.npcName} Died", Color.magenta, this);
            }

            if (removeAfterDeath)
            {
                removeAfterDeathTimer.Start();
            }
        }


        public float GetDamage()
        {
            return myStatsHandler.CurrentStats.damage;
        }

        public void StopMovement(float time)
        {
            if (time <= 0f)
            {
                return; // TODO: StartMovement
            }

            // TODO: stop for time using enumerator or the animator!
            if (!StopMovementTimer.IsSet || (StopMovementTimer.IsSet && StopMovementTimer.RemainingTime < time))
            {
                StopMovementTimer.Start(time);
            }

            if (debug)
            {
                Logger.Log($"Stopped Movement {time} {StopMovementTimer.IsSet}", this);
            }

            StopMovementHelper();
        }

        public void StartMovement()
        {
            StopMovementTimer.Clear();
            animationControls.CanMove = true;
        }

        public void SwitchMovement(float time)
        {
            if (animationControls.CanMove)
            {
                StopMovement(time);
            }
            else
            {
                StartMovement();
            }
        }

        public void SetDefaultDirection(Direction newDir)
        {
            DefaultDirection = newDir;
            HasDefaultDirection = true;
        }

        public void RemoveDefaultDirection()
        {
            HasDefaultDirection = false;
        }

        #endregion

        #region Private Methods

        protected virtual void RunWithoutAcceleration()
        {
            if (!ShouldMove)
            {
                return;
            }

            Velocity.x = DesiredVelocity.x;
            var minVelocity = 0.05f; // TODO: expose
            Velocity.x = Mathf.Abs(Velocity.x) < minVelocity ? 0f : Velocity.x;
            Velocity.y = MyRigidbody.velocity.y; // TODO: change to some gravity?
            MyRigidbody.velocity = Velocity;
        }

        protected bool ShouldSwitchDirectionWhenNotMoving()
        {
            var shouldSwitch = false;

            if (!useNotMovingTimer)
            {
                return false;
            }

            if (!animationControls.IsMoving)
            {
                if (!notMovingTimer.IsSet)
                {
                    notMovingTimer.Start();
                }
                else if (!notMovingTimer.IsActive)
                {
                    shouldSwitch = true;
                    notMovingTimer.Clear();
                }
            }
            else
            {
                notMovingTimer.Clear();
            }


            return shouldSwitch && animationControls.CanMove;
        }

        protected bool TakeDamage(float dmgTaken)
        {
            if (hurtInvTime.IsSet && hurtInvTime.IsActive)
            {
                return false;
            }
            hurtInvTime.Start();
            events.onHurt.Invoke();
            var newHp = myStatsHandler.TakeDamage(dmgTaken);
            if (newHp > 0)
            {
                StopDash();
                StopMovement(hurtTime);

                animationControls.Hurt = true;
            }
            else
            {
                Death();
            }

            return true;
        }

        [Button]
        protected void DebugDamage()
        {
            TakeDamage(5);
        }

        [Button]
        [ContextMenu("Log Stats")]
        protected void TestContextMenu()
        {
            Logger.Log(NpcDataScriptable, this);

            StopMovement(hurtTime);

            animationControls.Hurt = true;
        }

        private void StopMovementHelper()
        {
            // TODO: constraint x?
            MyRigidbody.velocity = Vector2.zero; // TODO: Slowdown gradually not immediate
            animationControls.CanMove = false;
            if (IsDashing)
            {
                StopDash();
            }
        }

        #endregion
        

    }

}
