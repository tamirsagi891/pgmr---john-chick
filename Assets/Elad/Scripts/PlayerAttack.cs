using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

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
    
    [Space(10)] [Header("Arrow Attack")]
    [SerializeField] private float arrowAttackCoolDownTime = 1f;
    private float _arrowAttackCoolDownTimer = 0;
    private bool _arrowAttackCoolDown = false;
    [SerializeField] private Transform arrowInstantiatePosition;
    [SerializeField] private GameObject arrowPrefab;
    
    
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

        if (_arrowAttackCoolDown)
        {
            _arrowAttackCoolDownTimer -= Time.deltaTime;
            if (_arrowAttackCoolDownTimer < 0)
            {
                _arrowAttackCoolDown = false;
            }
        }
        
    }

    public void OnAttack(InputAction.CallbackContext context)
    {

        if (context.started && !_attackCoolDown)
        {
            _animator.SetTrigger(AnimationStrings.attackTrigger);
            _attackTriggerResetTimer = arrowAttackCoolDownTime;
        }
        
    }
    
    public void OnArrowAttack(InputAction.CallbackContext context)
    {

        if (context.started && !_arrowAttackCoolDown)
        {
            _animator.SetTrigger(AnimationStrings.arrowAttack);
            _arrowAttackCoolDownTimer = arrowAttackCoolDownTime;
            _arrowAttackCoolDown = true;
        }
        
    }

    public void FireArrow()
    {
        Instantiate(arrowPrefab, arrowInstantiatePosition.position, arrowPrefab.transform.rotation);

    }
}
