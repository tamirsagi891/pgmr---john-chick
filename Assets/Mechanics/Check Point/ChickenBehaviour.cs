using UnityEngine;

public class ChickenBehaviour : MonoBehaviour
{
    [SerializeField] private float minIdleTime = 2f;
    [SerializeField] private float maxIdleTime = 5f;
    [SerializeField] private float minWalkTime = 1f;
    [SerializeField] private float maxWalkTime = 3f;
    [SerializeField] private float walkSpeed = 1f;
    [SerializeField] private float runSpeed = 2f;

    private Animator animator;
    private BoxCollider2D roamArea;
    private Rigidbody2D rb;
    private bool isWalking = false;
    private float timeToChange = 0f;
    private float direction;
    private float moveSpeed;

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
            MoveRandomly();

            if (Time.time >= timeToChange)
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
                direction = Random.Range(-1f, 1f);
                
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

                timeToChange = Time.time + Random.Range(minWalkTime, maxWalkTime);
            }
        }
    }

    private void MoveRandomly()
    {
        Vector2 newPosition = rb.position + new Vector2(direction * moveSpeed * Time.deltaTime, 0f) * 0.05f;

        // Check if the new position is inside the roam area
        if (roamArea.bounds.Contains(newPosition))
        {
            rb.MovePosition(newPosition);
            FlipBasedOnDirection(); // Flip based on direction
        }
        else
        {
            // If new position is outside the roam area, turn around
            direction = -direction;
            FlipBasedOnDirection(); // Flip based on direction
        }
    }

    private void FlipBasedOnDirection()
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
