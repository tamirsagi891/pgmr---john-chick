using System.Collections.Generic;
using Avrahamy;
using BitStrap;
using UnityEngine;
using UnityEngine.Serialization;
using Logger = Nemesh.Logger;

namespace Mechanics.Enemies
{
    [AddComponentMenu("NPC/Base Npc", -1)]
    [SelectionBase]
    [DisallowMultipleComponent]
    [RequireComponent(
        typeof(StatsHandler))] // TODO: create new class that just requires the Components in the right order
    [RequireComponent(typeof(Rigidbody2D))]
    public class BaseNpc : MonoBehaviour, IAttacker, ICanBeAttacked
    {

        #region Inspector

        [Header("Base NPC Fields")]
        [ContextMenuItem("Log Stats", "TestContextMenu")]
        [SerializeField]
        [Tooltip("The data this NPC will reference")]
        [InlineScriptableObject]
        protected NpcData myData;

        [SerializeField]
        [RequiredReference]
        protected NpcAnimationControls animationControls;

        [SerializeField]
        [RequiredReference]
        protected GameObject attackController; // TODO: MonoBehaviour of some type

        [Space]
        [HelpBox("THIS SHOULD BE FROM ANIMATION NOT HERE! But im too lazy.", HelpBoxAttribute.MessageType.Warning)]
        [SerializeField]
        private float attackTime = 1f;

        [SerializeField]
        private float hurtTime = 1f;

        [SerializeField]
        private PassiveTimer dashTime = new(0.2f);

        [SerializeField]
        [Tooltip("After how many second of not moving try to switch direction")]
        private PassiveTimer notMovingTimer = new(0.25f);


        [Space]
        [SerializeField]
        private bool stopMovementWhileTargetExists = true;

        [SerializeField]
        private bool detectEdges = true;

        [Space]
        [SerializeField]
        public NpcEvents events;

        [Space]
        [Header("Debug")]
        [SerializeField]
        private bool debug;

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

        public int GroundContacts
        {
            get => _groundContacts;
            set
            {
                _groundContacts = value;
                animationControls.IsGrounded = _groundContacts > 0;
                if (!animationControls.IsGrounded)
                {
                    // TODO: handle gravity like Elad did
                    // _velocity.y = -9.8f;
                }

                if (debug)
                {
                    Logger.Log($"{NpcData.npcName} Ground Contacts: {_groundContacts}", this);
                }
            }
        }

        public PlayerAttackController PlayerContact
        {
            get => _playerContact;
            set
            {
                _playerContact = value;
                // TODO: add script for attack strategy
                if (value != null)
                {
                    MovementBehaviour.EnabledBehaviour = false;
                    WalkTarget = value.transform;
                    events.onPlayerDetected.Invoke();
                }
                else
                {
                    MovementBehaviour.EnabledBehaviour = true;
                    MovementBehaviour.GoToNextPoint();
                }

                if (debug)
                {
                    Logger.Log($"{NpcData.npcName} Contact With Player: {_playerContact}", this);
                }
            }
        }

        public NpcData NpcData
        {
            get => myData;
            set => myData = value;
        }

        public List<ICanBeAttacked> AttackTargets
        {
            get => _attackTargets;
            set => _attackTargets = value;
        }

        public Transform WalkTarget
        {
            get => _walkTarget;
            set
            {
                _walkTarget = value;
                _hasDestination = _walkTarget != null;
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

        private int _groundContacts;

        private PlayerAttackController _playerContact; // TODO: change to any target?
        private List<ICanBeAttacked> _attackTargets = new();

        private StatsHandler _myStatsHandler;

        private INpcMovementBehaviour _movementBehaviour;
        private Transform _walkTarget; // TODO: create type of WalkTarget?
        private bool _hasDestination;

        private Rigidbody2D _myRigidbody;
        private float _desiredVelocityX;
        private Vector2 _velocity;
        private float _maxSpeedChange;
        private float _acceleration;
        private float _deceleration;
        private float _turnSpeed;

        private readonly PassiveTimer _attackCd = new();
        private readonly PassiveTimer _stopMovement = new();

        private bool _shouldMove;

        #endregion

        #region MonoBehaviour

        private void Awake()
        {
            _myStatsHandler = GetComponent<StatsHandler>();
            _myRigidbody = GetComponent<Rigidbody2D>();
            notMovingTimer.Clear();
        }

        private void Update()
        {
            if (dashTime.IsSet && !dashTime.IsActive)
            {
                dashTime.Clear();
                _myStatsHandler.CurrentStats.movementSpeed -= _myStatsHandler.CurrentStats.extraDashSpeed;
            }
            if (_stopMovement.IsSet && !_stopMovement.IsActive &&
                (!stopMovementWhileTargetExists || AttackTargets.Count <= 0))
            {
                StartMovement();
            }

            if (AttackTargets.Count == 0)
            {
                return;
            }

            StopMovement(attackTime);

            if (_attackCd.IsSet && _attackCd.IsActive) // TODO: this repeats - make it a method of PassiveTImer
            {
                return;
            }

            _attackCd.Start(_myStatsHandler.CurrentStats.cooldown);
            AttackAllTarget();
        }

        private void FixedUpdate()
        {
            animationControls.IsMoving =
                Mathf.Abs(_myRigidbody.velocity.x) > 0.05f; // TODO: optimize - report once and then dont.

            if (_hasDestination)
            {
                var shouldSwitch = ShouldSwitchDirectionWhenNotMoving();

                if (animationControls.IsGrounded) // TODO: Dash + Jump doesnt work cause of this
                {
                    var targetLeft = _walkTarget.position.x < transform.position.x;
                    animationControls.Direction =
                        targetLeft ? Direction.Left : Direction.Right; // TODO: min timeout for direction changes

                    var speed = _myStatsHandler.CurrentStats
                        .movementSpeed; //Mathf.Max(_myStatsHandler.CurrentStats.movementSpeed, 0f);
                    speed = animationControls.Direction == Direction.Left ? -speed : speed;
                    _desiredVelocityX = speed;

                    _shouldMove = Mathf.Abs(_desiredVelocityX) > 0.01f;

                    if (DetectEdges && EdgeInFront || shouldSwitch) // TODO: ignore edges in pursuit?
                    {

                        EdgeInFront = false;

                        _desiredVelocityX = 0f;
                        if (MovementBehaviour.EnabledBehaviour)
                        {
                            MovementBehaviour?.GoToNextPoint();
                        }
                        // TODO: else change direction

                        animationControls.SwitchDirection();

                    }
                }


                if (animationControls.CanMove)
                {
                    // var friction = 0f; // TODO: add friction to npc? and rotation and stuff
                    // _desiredVelocity = new Vector2( Mathf.Max(_myStatsHandler.CurrentStats.movementSpeed - friction, 0f), 0f);
                    RunWithoutAcceleration();
                }

            }
            else
            {
                _shouldMove = false;
            }
        }
        

        #endregion


        #region Public Methods

        [Button]
        public void Jump()
        {
            if (!animationControls.IsGrounded)
            {
                return;
            }

            _myRigidbody.AddForce(new Vector2(0f, _myStatsHandler.CurrentStats.jumpForce), ForceMode2D.Impulse);
            animationControls.Jump = true;
            events.onJump.Invoke();
        }
        
        [Button]
        public void Dash()
        {
            if (!animationControls.CanMove)
            {
                return;  // TODO: dash when not moving should still dash.
            }
            if (dashTime.IsSet && dashTime.IsActive)
            {
                return;
            }
            dashTime.Start();
            _myStatsHandler.CurrentStats.movementSpeed += _myStatsHandler.CurrentStats.extraDashSpeed;
            // TODO: Copy elad's implementation
            // var force = 20; // TODO: Dash force?
            // force = CurrentDirection == Direction.Left ? -force : force;
            // _myRigidbody.AddForce(new Vector2(force, 0f), ForceMode2D.Impulse); 
            animationControls.Dash = true;
            events.onDash.Invoke();
        }

        public bool Attack(ICanBeAttacked attackTarget) // TODO: type of object that takes damage instead of transform
        {
            // TODO: if can attack, valid start, etc...
            if (_attackCd.IsSet && _attackCd.IsActive)
            {
                return false;
            }

            StopMovement(attackTime);

            _attackCd.Start(_myStatsHandler.CurrentStats.cooldown);

            animationControls.Attack = true;

            attackTarget.Hurt(this);
            return true;
        }

        private void AttackAllTarget()
        {
            events.onAttack.Invoke();

            animationControls.Attack = true;
            foreach (var target in AttackTargets)
            {
                target.Hurt(this);
            }
        }


        public bool Hurt(IAttacker attacker)
        {
            // TODO: take damage
            var dmgTaken = attacker.GetDamage();

            if (dmgTaken <= 0)
            {
                return false;
            }

            events.onHurt.Invoke();

            StopMovement(hurtTime);

            animationControls.Hurt = true;
            return true;
        }

        public float GetDamage()
        {
            return _myStatsHandler.CurrentStats.damage;
        }

        [Button]
        [ContextMenu("Log Stats")]
        public void TestContextMenu()
        {
            Logger.Log(NpcData, this);

            StopMovement(hurtTime);

            animationControls.Hurt = true;
        }

        public void StopMovement(float time)
        {
            // TODO: stop for time using enumerator or the animator!
            if (!_stopMovement.IsSet || _stopMovement.RemainingTime < time)
            {
                _stopMovement.Start(time);
            }

            _myRigidbody.velocity = Vector2.zero;
            animationControls.CanMove = false;
        }

        public void StartMovement()
        {
            _stopMovement.Clear();
            animationControls.CanMove = true;
        }

        #endregion

        #region Private Methods

        private void RunWithoutAcceleration()
        {
            _velocity.x = _desiredVelocityX;
            var minVelocity = 0.05f; // TODO: expose
            _velocity.x = Mathf.Abs(_velocity.x) < minVelocity ? 0f : _velocity.x;
            _velocity.y = _myRigidbody.velocity.y; // TODO: change to some gravity?
            _myRigidbody.velocity = _velocity;

            // _velocity.y = animationControls.IsGrounded ? 0f : _myRigidbody.velocity.y; // TODO: change to some gravity?
            // _myRigidbody.MovePosition(_myRigidbody.position + _velocity * Time.fixedDeltaTime);
        }
        
        private bool ShouldSwitchDirectionWhenNotMoving()
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

        #endregion


    }
}
