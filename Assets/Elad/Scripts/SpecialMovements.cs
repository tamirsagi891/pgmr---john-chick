using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class SpecialMovements : MonoBehaviour
{
    [Space(10)] [Header("Dashing")] private bool _canDash = true;
    private bool _isDashing;
    [SerializeField] private float maxSpeedDashing = 24f;
    private float dashingTime = 0.2f;
    private float dashingCoolDown = 1f;
    private TrailRenderer _tr;

    [Space(10)] [Header("Dodge Roll")] private bool _canDodgeRoll = true;
    private bool _isDodgeRoll;
    [SerializeField] private float maxSpeedDodgeRoll = 24f;
    private float dodgeRollTime = 0.2f;
    private float dodgeRollCoolDown = 1f;

    [Space(10)] [Header("Components")] private TouchingDirection _touchingDirection;
    private Rigidbody2D _rB;
    private Animator _animator;
    private PlayerController _playerController;
    private float _originalGravity;
    private float _currentSpeed;
    
    public float CurrentSpeed
    {
        get
        {
            switch (_currentMovementStatus)
            {
                case MovementStatus.Dash:
                    return maxSpeedDashing;
                case MovementStatus.DodgeRoll:
                    return maxSpeedDodgeRoll;
            }

            return 0;
        }
    }

    public enum MovementStatus
    {
        None,
        Dash,
        DodgeRoll
    }

    private MovementStatus _currentMovementStatus = MovementStatus.None;

    public MovementStatus CurrentMovementStatus
    {
        get => _currentMovementStatus;

        private set => _currentMovementStatus = value;
    }
    
    private void Awake()
    {
        _rB = GetComponent<Rigidbody2D>();
        _originalGravity = _rB.gravityScale;
        _animator = GetComponent<Animator>();
        _tr = GetComponent<TrailRenderer>();
        _playerController = GetComponent<PlayerController>();
        _touchingDirection = GetComponent<TouchingDirection>();
    }


    public void OnDash(InputAction.CallbackContext context)
    {
        if (_touchingDirection.IsGrounded)
        {
            if (_currentMovementStatus == MovementStatus.None)
            {
                if (context.started && _canDash)
                {
                    StartCoroutine(Dash());
                }
            }
        }
    }

    private IEnumerator Dash()
    {
        _canDash = false;
        _isDashing = true;
        _rB.gravityScale = 0;
        _tr.emitting = true;
        _animator.SetTrigger(AnimationStrings.dashTrigger);
        _currentMovementStatus = MovementStatus.Dash;
        yield return new WaitForSeconds(dashingTime);

        _tr.emitting = false;
        _rB.gravityScale = _originalGravity;
        _isDashing = false;
        _currentMovementStatus = MovementStatus.None;

        yield return new WaitForSeconds(dashingCoolDown);

        _canDash = true;
    }

    public void OnDodgeRoll(InputAction.CallbackContext context)
    {
        if (_touchingDirection.IsGrounded)
        {
            if (_currentMovementStatus == MovementStatus.None)
            {
                if (context.started && _canDodgeRoll)
                {
                    StartCoroutine(DodgeRoll());
                }
            }
        }
    }

    private IEnumerator DodgeRoll()
    {
        _canDodgeRoll = false;
        _isDodgeRoll = true;
        _animator.SetTrigger(AnimationStrings.dodgeRollTrigger);
        CurrentMovementStatus = MovementStatus.DodgeRoll;
        _playerController.ChangeCollider(PlayerController.ColliderKind.DodgeRoll);

        yield return new WaitForSeconds(dodgeRollTime);

        _isDodgeRoll = false;
        CurrentMovementStatus = MovementStatus.None;
        _playerController.ChangeCollider(PlayerController.ColliderKind.Capsule);

        yield return new WaitForSeconds(dodgeRollCoolDown);
        _canDodgeRoll = true;
    }
}