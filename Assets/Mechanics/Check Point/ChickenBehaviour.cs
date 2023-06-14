using UnityEngine;

public class ChickenBehaviour : MonoBehaviour
{
    [SerializeField] private float minIdleTime = 2f;
    [SerializeField] private float maxIdleTime = 5f;
    [SerializeField] private float walkSpeed = 1f;
    [SerializeField] private float runSpeed = 2f;
    [SerializeField] private float minStepDistance = 0.5f;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float minJumpInterval = 2f;
    [SerializeField] private float maxJumpInterval = 5f;
    [SerializeField] private float initialJumpDelay = 3f; // Delay before first jump
    [SerializeField] private LayerMask groundLayer; // Layer that defines what is ground

    private Animator animator;
    private BoxCollider2D roamArea;
    private Rigidbody2D rb;
    private bool isWalking = false;
    private float timeToChange = 0f;
    private Vector2 targetPosition;
    private float moveSpeed;
    private float proximityThreshold = 0.1f;
    private float nextJumpTime;
    private bool isGrounded = false;
    private bool isJumping = false;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        roamArea = transform.parent.GetComponent<BoxCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 1f; // Adjust this to control the falling speed
        nextJumpTime = Time.time + initialJumpDelay; // Set the initial delay for the first jump
    }

    private void Update()
    {
        if (isWalking && isGrounded)
        {
            MoveToTarget();

            if (Vector2.Distance(rb.position, targetPosition) < proximityThreshold)
            {
                isWalking = false;
                animator.SetBool("isWalking", isWalking);
                animator.speed = 1f;
                timeToChange = Time.time + Random.Range(minIdleTime, maxIdleTime);
            }
        }
        else
        {
            if (Time.time >= timeToChange && isGrounded)
            {
                isWalking = true;
                animator.SetBool("isWalking", isWalking);

                float minLimit = Mathf.Max(roamArea.bounds.min.x, rb.position.x - minStepDistance);
                float maxLimit = Mathf.Min(roamArea.bounds.max.x, rb.position.x + minStepDistance);
                float x = Random.Range(minLimit, maxLimit);
                targetPosition = new Vector2(x, rb.position.y);

                if (Random.value > 0.5f)
                {
                    moveSpeed = walkSpeed;
                    animator.speed = 1f;
                }
                else
                {
                    moveSpeed = runSpeed;
                    animator.speed = 2f;
                }
            }
        }

        if (isGrounded && !isJumping && Time.time >= nextJumpTime)
        {
            Jump();
            nextJumpTime = Time.time + Random.Range(minJumpInterval, maxJumpInterval); // Randomize the interval until the next jump
        }
    }

    private void MoveToTarget()
    {
        Vector2 newPosition = Vector2.MoveTowards(rb.position, targetPosition, moveSpeed * Time.deltaTime);

        float direction = (newPosition.x >= rb.position.x) ? 1 : -1;
        FlipBasedOnDirection(direction);

        rb.MovePosition(newPosition);
    }

    private void FlipBasedOnDirection(float direction)
    {
        if (direction < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
        else if (direction > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
    }

    private void Jump()
    {
        isJumping = true;
        rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (((1 << collision.gameObject.layer) & groundLayer) != 0)
        {
            isGrounded = true;
            isJumping = false;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (((1 << collision.gameObject.layer) & groundLayer) != 0)
        {
            isGrounded = false;
        }
    }
}
