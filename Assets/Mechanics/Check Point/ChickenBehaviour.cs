using UnityEngine;

public class ChickenBehaviour : MonoBehaviour
{
    [SerializeField] private float minIdleTime = 2f;
    [SerializeField] private float maxIdleTime = 5f;
    [SerializeField] private float walkSpeed = 1f;
    [SerializeField] private float runSpeed = 2f;
    [SerializeField] private float minStepDistance = 0.5f; // Minimum step distance

    private Animator animator;
    private BoxCollider2D roamArea;
    private Rigidbody2D rb;
    private bool isWalking = false;
    private float timeToChange = 0f;
    private Vector2 targetPosition;
    private float moveSpeed;
    private float proximityThreshold = 0.1f;  // Threshold to consider the chicken close enough to the target

    private void Awake()
    {
        animator = GetComponent<Animator>();
        roamArea = transform.parent.GetComponent<BoxCollider2D>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (isWalking)
        {
            MoveToTarget();

            // Check if chicken reached the target
            if (Vector2.Distance(rb.position, targetPosition) < proximityThreshold)
            {
                isWalking = false;
                animator.SetBool("isWalking", isWalking);
                animator.speed = 1f; // Reset the animator speed
                timeToChange = Time.time + Random.Range(minIdleTime, maxIdleTime);
            }
        }
        else
        {
            if (Time.time >= timeToChange)
            {
                isWalking = true;
                animator.SetBool("isWalking", isWalking);
                
                // Set a random target within the roam area in global coordinates, while respecting the minimum step distance
                float minLimit = Mathf.Max(roamArea.bounds.min.x, rb.position.x - minStepDistance);
                float maxLimit = Mathf.Min(roamArea.bounds.max.x, rb.position.x + minStepDistance);
                float x = Random.Range(minLimit, maxLimit);
                targetPosition = new Vector2(x, rb.position.y);

                // Randomly decide if the chicken should walk or run
                if (Random.value > 0.5f)
                {
                    moveSpeed = walkSpeed;
                    animator.speed = 1f; // Set animation speed for walking
                }
                else
                {
                    moveSpeed = runSpeed;
                    animator.speed = 2f; // Set animation speed for running
                }
            }
        }
    }

    private void MoveToTarget()
    {
        Vector2 newPosition = Vector2.MoveTowards(rb.position, targetPosition, moveSpeed * Time.deltaTime);

        // Update the direction based on the new position
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
}
