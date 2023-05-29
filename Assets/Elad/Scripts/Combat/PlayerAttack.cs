using System;
using System.Collections;
using System.Collections.Generic;
using Elad.Scripts;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Pool;

[RequireComponent(typeof(FeathersManager))]
public class PlayerAttack : MonoBehaviour
{
    [Header("Components")] private Rigidbody2D _rB;
    private Animator _animator;

    [Space(10)] [Header("Ground Attack")] [SerializeField]
    private float attackTriggerResetTime = 0.25f;

    private float _attackTriggerResetTimer = 0;

    [SerializeField] private float groundAttackCoolDownTime = 1f;
    private float _groundAttackCoolDownTimer = 0;
    private bool _attackCoolDown = false;
    
    
    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }
    

    private void Update()
    {
        if (_attackTriggerResetTimer > 0)
        {
            _attackTriggerResetTimer -= Time.deltaTime;
            if (_attackTriggerResetTimer < 0)
            {
                _attackTriggerResetTimer = 0;
                _animator.ResetTrigger(AnimationStrings.attackTrigger);
            }
        }

        if (!_attackCoolDown)
        {
            _attackCoolDown = _animator.GetBool(AnimationStrings.attackCoolDown);
            _groundAttackCoolDownTimer = groundAttackCoolDownTime;
        }

        if (_attackCoolDown)
        {
            _groundAttackCoolDownTimer -= Time.deltaTime;
            if (_groundAttackCoolDownTimer < 0)
            {
                _animator.SetBool(AnimationStrings.attackCoolDown, false);
                _attackCoolDown = false;
            }
        }
        
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.started && !_attackCoolDown)
        {
            _animator.SetTrigger(AnimationStrings.attackTrigger);
            _attackTriggerResetTimer = attackTriggerResetTime;
        }
    }
    
}