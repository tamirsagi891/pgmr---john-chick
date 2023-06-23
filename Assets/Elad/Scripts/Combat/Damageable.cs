using System;
using BitStrap;
using Elad.Events;
using Elad.Save_Load_System;
using Managers;
using Mechanics.Enemies;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using Logger = Nemesh.Logger;

namespace Elad.Scripts.Combat
{
    public class Damageable : MonoBehaviour, ICanBeAttacked
    {
        [SerializeField]
        [Tooltip("The code to open the game over or go to the last check point is in the base menu controller")]
        private int checkPointsLives = 3;

        private int _deathAmounts = 0;

        private bool _dieButInGlide;

        [Header("Times")]
        [Header("Components")] private Animator _animator;
        private SpriteRenderer _spriteRenderer;

        [Header("Amounts")] [SerializeField] private int initialHealth = 100;
        [SerializeField] private int maxHealth = 100;

        [SerializeField] [Tooltip("Do we want the option of minimum life after revive")] private bool useMinLifeInRevived = true;
        [SerializeField]
        [Tooltip("The minimum amount of live the player can have when revive")]
        private int minLifeInRevived = 2;

        public UnityEvent<int, Vector2, float> damageableHit;

        public int MaxHealth
        {
            get => maxHealth;
            set => maxHealth = value;
        }

        [SerializeField] [ReadOnly] private int _curHealth;

        public int Health
        {
            get => _curHealth;
            set
            {
                // Logger.Log("in health : " + value);
                if (value != _curHealth)
                {
                    _curHealth = value;
                    if (_curHealth <= 0)
                    {
                        _curHealth = 0;
                        IsAlive = false;
                    }

                    PlayerSaveData.health = _curHealth;
                }
            }
        }

        [Header("States")] [SerializeField] private bool isInvincible;

        public bool IsInvincible
        {
            get => isInvincible;
            set
            {
                // Logger.Log("player Died -1");
                PlayerStatus.PlayerIsInvincible = value;
                isInvincible = value;
                _blinkTimer = blinkTime;
                if (!value)
                {
                    // Logger.Log("player Died 0");
                    // Logger.Log("stop being IsInvincible");
                    _spriteRenderer.color = _originalColor;
                }
            }
        }

        [SerializeField] private bool isInvincibleTest;
        [SerializeField] private bool isAlive = true;

        public bool IsAlive
        {
            get => isAlive;
            set
            {
                isAlive = value;
                _animator.SetBool(AnimationStrings.isAlive, value);
                if (!value && !_dieButInGlide)
                {
                    if (PlayerStatus.IsGliding)
                    {
                        _dieButInGlide = true;
                    }

                    else
                    {
                        Logger.Log("in IsAlive");
                        characterEvents.PlayerDied.Invoke();
                        _dieButInGlide = false;
                    }
                }
            }
        }


        public void RevivePlayer()
        {
            IsAlive = true;
        }

        public bool LockVelocity
        {
            get { return _animator.GetBool(AnimationStrings.lockVelocity); }


            set { _animator.SetBool(AnimationStrings.lockVelocity, value); }
        }


        [Header("Time")] [SerializeField] private float invincibilityTimer = 0.25f;
        private float timeSinceHit = 0;

        [Space(3)] [Header("Hit Blinking")] [SerializeField]
        private Color blinkColor;

        private Color _originalColor;
        [SerializeField] private float blinkTime = 0.01f;
        private float _blinkTimer;
        private bool _inOriginalColor = true;
        private TouchingDirection _touchingDirection;

        private PlayerSaveData PlayerSaveData
        {
            get => PlayerStatus.PlayerSaveData;
            set => PlayerStatus.PlayerSaveData = value;
        }

        public int CheckPointsLives
        {
            get => checkPointsLives;
            set => checkPointsLives = value;
        }

        public int DeathAmounts
        {
            get => _deathAmounts;
            set => _deathAmounts = value;
        }


        [Header("Tests")] [SerializeField] private int setLifeValue = 50;

        private void OnEnable()
        {
            characterEvents.FunctionsSave.AddListener(SavePlayerStatus);
            characterEvents.FunctionsLoad.AddListener(LoadPlayerStatus);
        }

        private void OnDisable()
        {
            characterEvents.FunctionsSave.RemoveListener(SavePlayerStatus);
            characterEvents.FunctionsLoad.RemoveListener(LoadPlayerStatus);
        }

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _originalColor = _spriteRenderer.color;
            _curHealth = maxHealth;
            _animator = GetComponent<Animator>();
            PlayerStatusSetVariables();

            PlayerSaveData.health = _curHealth;
        }

        private void Start()
        {
            SavePlayerStatus();
        }

        private void Update()
        {
            
            if (_dieButInGlide)
            {
                if (!_touchingDirection)
                {
                    _touchingDirection = PlayerStatus.Player.GetComponent<TouchingDirection>();
                }
                
                if (_touchingDirection.IsGrounded)
                {
                    characterEvents.PlayerDied.Invoke();
                    _dieButInGlide = false;
                }
                 
                return;    
            }
            
            if (IsInvincible)
            {
                if (timeSinceHit > invincibilityTimer)
                {
                    IsInvincible = false;
                    timeSinceHit = 0;
                }

                timeSinceHit += Time.deltaTime;
                Blink();
            }


            if (isInvincibleTest)
            {
                isInvincibleTest = false;
                IsInvincible = true;
            }

            
        }

        [Button]
        public void Kill()
        {
            IsAlive = false;
            Logger.Log("In kill function");
            // characterEvents.PlayerDied.Invoke();
        }

        private void Blink()
        {
            if (!IsAlive)
            {
                isInvincible = false;
                FinishBlink();
            }
            
            if (isInvincible)
            {
                _blinkTimer -= Time.deltaTime;
                if (_blinkTimer <= 0)
                {
                    _blinkTimer = blinkTime;

                    if (_inOriginalColor)
                    {
                        _spriteRenderer.color = blinkColor;
                        _inOriginalColor = false;
                    }
                    else
                    {
                        _spriteRenderer.color = _originalColor;
                        _inOriginalColor = true;
                    }
                }
            }
            else
            {
                FinishBlink();
            }
        }

        private void FinishBlink()
        {
            _spriteRenderer.color = _originalColor;
            _inOriginalColor = true;
        }
        


        public bool GotHit(int damage, Vector2 knockBack, float knockBackDelay = 0f)
        {
            if (GeneralGameManager.IsGamePause) return false;
            if (IsAlive && !IsInvincible)
            {
                Health -= damage;
                IsInvincible = true;
                LockVelocity = true;
                _animator.SetTrigger(AnimationStrings.hitTrigger);
                damageableHit?.Invoke(damage, knockBack, knockBackDelay);
                PlayerStatusSetVariables();
                characterEvents.CharacterDamaged.Invoke(gameObject, damage);
                return true;
            }


            return false;
        }

        public bool AddLife(int healAmount)
        {
            if (IsAlive)
            {
                Health = Mathf.Min(Health + healAmount, maxHealth);
                PlayerStatusSetVariables();
                characterEvents.CharacterHealed.Invoke(gameObject, healAmount);
                return true;
            }

            return false;
        }

        private void PlayerStatusSetVariables()
        {
            if (gameObject.CompareTag(TagStrings.playerTag))
            {
                PlayerStatus.maxHealth = maxHealth;
                PlayerStatus.curHealth = Health;
            }
        }

        public bool Hurt(AttackParameters attackParameters)
        {
            Logger.Log($"Attacked by {attackParameters}", this);
            switch (attackParameters.Type)
            {
                case AttackType.Pickup:
                    Logger.Log("TODO: Stop Movement, Set Follow Target to attackParameters.FollowTransform",
                        Color.red, this);
                    return false;
                case AttackType.Shot:
                case AttackType.Regular:
                default:
                    return GotHit(1, attackParameters.KnockBack, attackParameters.KnockBackDelay);
                // return GotHit((int) attackParameters.Damage, attackParameters.KnockBack);
            }
        }

        public void SavePlayerStatus()
        {
            SaveGameOnJson.CurrentSaveData.playerSaveData = PlayerSaveData;
        }

        public void LoadPlayerStatus()
        {
            PlayerSaveData = SaveGameOnJson.CurrentSaveData.playerSaveData;
            Health = PlayerSaveData.health;
            PlayerStatus.curHealth = Health;
            SetMinLifeInRevive();

        }

        private void SetMinLifeInRevive()
        {
            if (Health < minLifeInRevived)
            {
                Health = minLifeInRevived;
            }
        }

        public Transform GetTransform() => transform;

        [Button]
        public void SetLife()
        {
            Health = setLifeValue;
        }
    }
}