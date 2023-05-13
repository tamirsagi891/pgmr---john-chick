using System;
using System.Collections;
using System.Collections.Generic;
using Avrahamy;
using BitStrap;
using UnityEngine;
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
        protected GameObject attackController; // TODO: MonoBehaviour of some type

        [Space]
        [HelpBox("THIS SHOULD BE FROM ANIMATION NOT HERE! But im too lazy.",
            HelpBoxAttribute.MessageType.Warning)]
        [SerializeField]
        protected float attackTime = 1f;

        [SerializeField]
        protected float hurtTime = 1f;

        [SerializeField]
        protected PassiveTimer dashTime = new(0.2f);

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

        [Space]
        [SerializeField]
        protected bool removeAfterDeath;

        [SerializeField]
        protected PassiveTimer removeAfterDeathTimer = new(10f);

        [Space]
        [SerializeField]
        public NpcEvents events;

        [Space]
        [Header("Debug")]
        [SerializeField]
        protected bool debug;

        #endregion

        #region Public Properties

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

        public PlayerAttackController PlayerContact
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
                if (value != null)
                {
                    WalkTarget = value.transform;
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

        public NpcDataScriptable NpcDataScriptable
        {
            get => myData;
            set => myData = value;
        }

        public List<ICanBeAttacked> AttackTargets { get; set; } = new();

        public Transform WalkTarget
        {
            get => _walkTarget;
            set
            {
                _walkTarget = value;
                HasDestination = _walkTarget != null;
            }
        }

        public bool EdgeInFront { get; set; }

        public bool DetectEdges
        {
            get => detectEdges;
            set => detectEdges = value;
        }

        public Direction CurrentDirection => animationControls.Direction;
        
        #endregion

        #region Private Fields

        private PlayerAttackController _playerContact; // TODO: change to any target?

        protected StatsHandler MyStatsHandler;
        private INpcMovementBehaviour _movementBehaviour;
        private Transform _walkTarget; // TODO: create type of WalkTarget?
        protected bool HasDestination;

        protected Rigidbody2D MyRigidbody;
        protected Vector2 DesiredVelocity;
        protected Vector2 Velocity;

        protected readonly PassiveTimer AttackCdTimer = new();
        protected readonly PassiveTimer StopMovementTimer = new();

        protected bool ShouldMove = true;
        protected bool IsDead;

        protected Direction defaultDirection;
        protected bool hasDefaultDirection;

        #endregion

        #region MonoBehaviour

        protected void Awake()
        {
            MyStatsHandler = GetComponent<StatsHandler>();
            MyRigidbody = GetComponent<Rigidbody2D>();
            notMovingTimer.Clear();
        }

        protected void OnDisable()
        {
            if (debug)
            {
                Logger.Log($"{NpcDataScriptable.npcName} Removed", Color.yellow, this);
            }

            events.onDisable.Invoke(this);
        }

        protected void Update()
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
                dashTime.Clear();
                MyStatsHandler.CurrentStats.movementSpeed -= MyStatsHandler.CurrentStats.extraDashSpeed;
                animationControls.StopDirectionSwitch = false;
            }

            if (!IsDead && StopMovementTimer.IsSet && !StopMovementTimer.IsActive &&
                (!stopMovementWhileTargetExists || AttackTargets.Count <= 0))
            {
                StartMovement();
            }

            return false;
        }

        protected void HandleAttackUpdate()
        {
            if (AttackTargets.Count == 0 || !canAirAttack && !IsGrounded)
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
                    var targetLeft = _walkTarget.position.x < transform.position.x;
                    animationControls.Direction = targetLeft ? Direction.Left : Direction.Right;

                    var speed = MyStatsHandler.CurrentStats.movementSpeed;
                    speed = animationControls.Direction == Direction.Left ? -speed : speed;
                    DesiredVelocity.x = speed;

                    ShouldMove = Mathf.Abs(DesiredVelocity.x) > 0.01f;

                    if ((DetectEdges && EdgeInFront) || shouldSwitch) // TODO: ignore edges in pursuit?
                    {
                        HandleDirectionSwitch();
                    }
                }
                
                RunWithoutAcceleration();
            }
            else if (hasDefaultDirection)
            {
                animationControls.Direction = defaultDirection;
            }
        }

        protected void AttackAllTarget()
        {
            events.onAttack.Invoke();

            foreach (var target in AttackTargets)
            {
                target.Hurt(this);
            }
        }

        #endregion


        #region Public Methods

        public void HandleDirectionSwitch()
        {
            EdgeInFront = false;

            DesiredVelocity.x = 0f;
            if (MovementBehaviour is { EnabledBehaviour: true })
            {
                MovementBehaviour?.GoToNextPoint();
            }

            animationControls.SwitchDirection();
        }

        [Button]
        public void Jump()
        {
            if (!IsGrounded)
            {
                return;
            }

            MyRigidbody.AddForce(new Vector2(0f, MyStatsHandler.CurrentStats.jumpForce), ForceMode2D.Impulse);
            animationControls.Jump = true;
            events.onJump.Invoke();
        }

        [Button]
        public void Dash()
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
            MyStatsHandler.CurrentStats.movementSpeed += MyStatsHandler.CurrentStats.extraDashSpeed;
            // TODO: Copy elad's implementation
            animationControls.Dash = true;
            events.onDash.Invoke();
        }

        public bool Attack(ICanBeAttacked attackTarget)
        {
            // TODO: if can attack, valid start, etc...
            if (AttackCdTimer.IsSet && AttackCdTimer.IsActive)
            {
                return false;
            }

            StopMovement(attackTime);

            AttackCdTimer.Start(MyStatsHandler.CurrentStats.cooldown);

            animationControls.Attack = true;

            attackTarget.Hurt(this);
            return true;
        }

        public void Attack()
        {
            StopMovement(attackTime);
            animationControls.Attack = true;
            AttackCdTimer.Start(MyStatsHandler.CurrentStats.cooldown);
            events.onAttackStart.Invoke();

            StartCoroutine(DelayExecution(attackTime, AttackAllTarget));
        }


        public bool Hurt(IAttacker attacker)
        {
            var dmgTaken = attacker.GetDamage();

            if (dmgTaken <= 0)
            {
                return false;
            }

            return TakeDamage(dmgTaken);
        }

        [Button]
        public void Death()
        {
            IsDead = true;
            animationControls.IsDead = true;

            StopMovementHelper();
            MyRigidbody.gravityScale = 1f;
            MyRigidbody.constraints |= RigidbodyConstraints2D.FreezePositionX;  // TODO: do we want to?

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
            return MyStatsHandler.CurrentStats.damage;
        }

        public void StopMovement(float time)
        {
            // TODO: stop for time using enumerator or the animator!
            if (!StopMovementTimer.IsSet || StopMovementTimer.RemainingTime < time)
            {
                StopMovementTimer.Start(time);
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
            defaultDirection = newDir;
            hasDefaultDirection = true;
        }

        public void RemoveDefaultDirection()
        {
            hasDefaultDirection = false;
        }

        #endregion

        #region Private Methods

        protected virtual void RunWithoutAcceleration()
        {
            Velocity.x = DesiredVelocity.x;
            var minVelocity = 0.05f; // TODO: expose
            Velocity.x = Mathf.Abs(Velocity.x) < minVelocity ? 0f : Velocity.x;
            Velocity.y = MyRigidbody.velocity.y; // TODO: change to some gravity?
            MyRigidbody.velocity = Velocity;
        }

        protected bool ShouldSwitchDirectionWhenNotMoving()
        {
            var shouldSwitch = false;
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
            events.onHurt.Invoke();
            var newHp = MyStatsHandler.TakeDamage(dmgTaken);
            if (newHp > 0)
            {
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
        }

        #endregion


        #region Coroutines

        public static IEnumerator DelayExecution(float delay, Action method, Func<bool> predicate)
        {
            yield return new WaitForSeconds(delay);
            if (predicate())
            {
                method();
            }
        }
        
        public static IEnumerator DelayExecution(float delay, Action method)
        {
            yield return new WaitForSeconds(delay);
            method();
        }


        #endregion
    }
}