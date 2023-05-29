using System;
using System.Collections;
using System.Collections.Generic;
using Elad.Scripts;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Pool;

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

    [Space(10)] [Header("Arrow Attack")] [SerializeField]
    private float arrowAttackCoolDownTime = 1f;

    private float _arrowAttackCoolDownTimer = 0;
    private bool _arrowAttackCoolDown = false;
    [SerializeField] private GameObject arrowInstantiatePosition;
    [SerializeField] private GameObject arrowPrefab;
    private LinkedPool<Arrow> _arrowPool;
    private int _startPoolSize = 50;
    [SerializeField] private int _maxPoolSize = 100;
    [SerializeField] private bool usePool = true;

    public bool UsePool
    {
        get => usePool;
        set => usePool = value;
    }

    private void Awake()
    {
        if (usePool)
        {
            _arrowPool = new LinkedPool<Arrow>(() => Instantiate(arrowPrefab,
                    arrowInstantiatePosition.transform.position,
                    arrowPrefab.transform.rotation).GetComponent<Arrow>(),
                GetArrow,
                arrow => arrow.gameObject.SetActive(false),
                arrow => Destroy(arrow.gameObject),
                false,
                _maxPoolSize
            );
        }

        _animator = GetComponent<Animator>();
    }
    

    private void GetArrow(Arrow arrow)
    {
        arrow.gameObject.SetActive(true);
        arrow.transform.position = arrowInstantiatePosition.transform.position;
        arrow.MyArrowData = PlayerStatus.CurrentArrowDataData;
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
        Arrow arrow;
        if (UsePool)
        {
            arrow = _arrowPool.Get();
        }

        else
        {
            arrow = Instantiate(arrowPrefab, arrowInstantiatePosition.transform.position,
                arrowPrefab.transform.rotation).GetComponent<Arrow>();
        }

        arrow.PlayerAttack = this;
        arrow.transform.position = arrowInstantiatePosition.transform.position;
        arrow.Fire();
    }

    public bool ReturnArrowToPoll(Arrow arrow)
    {
        if (UsePool)
        {
            _arrowPool.Release(arrow);
            return true;
        }

        return false;
    }
}