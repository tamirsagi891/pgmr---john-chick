using BitStrap;
using Elad.Events;
using Elad.Save_Load_System;
using Mechanics.Enemies;
using UnityEngine;
using UnityEngine.Events;
using Logger = Nemesh.Logger;

namespace Elad.Scripts.Combat
{
    public class Damageable : MonoBehaviour, ICanBeAttacked
    {
        [Header("Times")] private float timeToOpenGameOverMenu = 2f;
        [Header("Components")] private Animator _animator;
        private SpriteRenderer _spriteRenderer;

        [Header("Amounts")] [SerializeField] private int initialHealth = 100;
        [SerializeField] private int maxHealth = 100;


        public UnityEvent<int, Vector2> damageableHit;

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
                if (value != _curHealth)
                {
                    _curHealth = value;
                    if (_curHealth <= 0)
                    {
                        _curHealth = 0;
                        IsAlive = false;
                    }

                    _playerSaveData.health = _curHealth;
                }
            }
        }

        [Header("States")] [SerializeField] private bool isInvincible;

        public bool IsInvincible
        {
            get => isInvincible;
            set
            {
                Logger.Log("player Died -1");
                PlayerStatus.PlayerIsInvincible = value;
                isInvincible = value;
                _blinkTimer = blinkTime;
                if (!value)
                {
                    Logger.Log("player Died 0");
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
                if (!value)
                {
                    Logger.Log("in IsAlive");
                    characterEvents.PlayerDied.Invoke();
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

        private PlayerSaveData _playerSaveData
        {
            get => PlayerStatus._playerSaveData;
            set => PlayerStatus._playerSaveData = value;
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
        }

        private void Update()
        {
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
            // characterEvents.PlayerDied.Invoke();
        }

        private void Blink()
        {
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
                _spriteRenderer.color = _originalColor;
                _inOriginalColor = true;
            }
        }


        public bool GotHit(int damage, Vector2 knockBack)
        {
            if (IsAlive && !IsInvincible)
            {
                Health -= damage;
                IsInvincible = true;
                LockVelocity = true;
                _animator.SetTrigger(AnimationStrings.hitTrigger);
                damageableHit?.Invoke(damage, knockBack);
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
                    break;
                case AttackType.Shot:
                case AttackType.Regular:
                default:
                    return GotHit((int) attackParameters.Damage, attackParameters.KnockBack);
            }
        }

        public void SavePlayerStatus()
        {
            SaveGameOnJson.CurrentSaveData.playerSaveData = _playerSaveData;
        }

        public void LoadPlayerStatus()
        {
            _playerSaveData = SaveGameOnJson.CurrentSaveData.playerSaveData;

            Health = _playerSaveData.health;
        }

        public Transform GetTransform() => transform;

        [Button]
        public void SetLife()
        {
            Health = setLifeValue;
        }
    }
}