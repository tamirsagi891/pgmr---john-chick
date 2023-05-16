using System;
using System.Collections;
using System.Collections.Generic;
using BitStrap;
using Mechanics.Enemies;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class Damageable : MonoBehaviour
{
    [Header("Components")]
    private Animator _animator;
    
    [Header("Amounts")]
    [SerializeField] private int initialHealth = 100;
    [SerializeField] private int maxHealth = 100;

    public UnityEvent<int, Vector2> damageableHit;
    public int MaxHealth
    {
        get => maxHealth;
        set => maxHealth = value;
    }

    [SerializeField]
    [ReadOnly]
    private int _health;

    public int Health
    {
        get => _health;
        set
        {
            _health = value;
            if (_health <= 0)
            {
                IsAlive = false;
            }
        }
    }

    [Header("States")]
    [SerializeField] private bool isInvincible;

    public bool IsInvincible
    {
        get => isInvincible;
        set => isInvincible = value;
    }
    
    [SerializeField] private bool isAlive = true;

    public bool IsAlive
    {
        get => isAlive;
        set
        {
            isAlive = value;
            _animator.SetBool(AnimationStrings.isAlive ,value);
        }
    }
    
    public bool LockVelocity
    {
        get
        {
            return _animator.GetBool(AnimationStrings.lockVelocity);
        }


        set
        {
            _animator.SetBool(AnimationStrings.lockVelocity, value);
        }
    }
    

    [Header("Time")]
    [SerializeField] private float invincibilityTimer = 0.25f;
    private float timeSinceHit = 0;
    
    private void Awake()
    {
        _health = maxHealth;
        _animator = GetComponent<Animator>();
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
        }
        
    }

    public bool Hit(int damage, Vector2 knockBack)
    {
        if (IsAlive && !IsInvincible)
        {
            Health -= damage;
            IsInvincible = true;
            LockVelocity = true;
            _animator.SetTrigger(AnimationStrings.hitTrigger);
            damageableHit?.Invoke(damage, knockBack);

            characterEvents.CharacterDamaged.Invoke(gameObject, damage);
            return true;
        }

        return false;
    }

    public bool AddLife(int healAmount)
    {
        if (IsAlive)
        {
            Health += healAmount;
            characterEvents.CharacterHealed.Invoke(gameObject, healAmount);
            return true;
        }

        return false;
    }
}