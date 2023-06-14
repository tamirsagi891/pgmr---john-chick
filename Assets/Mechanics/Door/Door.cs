using System;
using System.Collections;
using BitStrap;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(BoxCollider2D))]
public class Door : MonoBehaviour
{
    [Serializable] private enum Direction { Up, Down, Left, Right }
    
    [Header("Direction and Time")]
    [SerializeField] private Direction direction;
    [SerializeField] private float distance = 4f;

    [SerializeField]
    private bool stayOpenUntilCommand;
    
    [Range(0.1f, 5f)][SerializeField] private float doorStayOpenTime = 5f; // time the door stays open
    [SerializeField] private float openTime = 0.5f; // time it takes for the door to open and close
    
    [Space(20)]
    [Header("Shake Effect")]
    [SerializeField] private float shakeDuration = 0.5f; // time for the shaking effect
    [SerializeField] private float shakeMagnitude = 0.1f; // strength of the shaking effect
    
    public bool IsDoorMoving => isDoorMoving;

    
    private Vector2 originalPosition;
    private bool isDoorMoving;
    private bool _canClose;


    private void Start()
    {
        originalPosition = transform.position;
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            OpenDoor();
            print("Door open");
        }
    }

    [Button]
    public bool OpenDoor()
    {
        // Only open the door if it's not currently moving
        if (!isDoorMoving)
        {
            StartCoroutine(DoorMovement());
            return true;
        }

        return false;
    }

    [Button]
    public bool CloseDoor()
    {
        // Only open the door if it's not currently moving
        if (!stayOpenUntilCommand)
        {
            return false;
        }

        _canClose = true;
        return true;
    }
    
    public void CloseDoorImmediate()
    {
        StopAllCoroutines();
        transform.position = originalPosition;
        isDoorMoving = false;
        _canClose = false;
    }

    private Vector2 GetDirectionVector(Direction direction)
    {
        switch (direction)
        {
            case Direction.Up:
                return Vector2.up;
            case Direction.Down:
                return Vector2.down;
            case Direction.Left:
                return Vector2.left;
            case Direction.Right:
                return Vector2.right;
            default:
                return Vector2.up;
        }
    }

    private IEnumerator ShakeEffect()
    {
        float elapsed = 0.0f;
        var startPos = transform.position;

        while (elapsed < shakeDuration)
        {
            float x = Random.Range(-1f, 1f) * shakeMagnitude;
            float y = Random.Range(-1f, 1f) * shakeMagnitude;

            transform.position = new Vector2(startPos.x + x, startPos.y + y);

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = originalPosition;
    }

    private IEnumerator DoorMovement()
    {
        isDoorMoving = true;

        Vector2 directionVector = GetDirectionVector(direction);
        Vector2 endPosition = originalPosition + directionVector * distance;
        float elapsedTime = 0;

        // Open the door
        while (elapsedTime < openTime)
        {
            float sinInterpolation = 0.5f * (1 - Mathf.Cos(Mathf.PI * elapsedTime / openTime));
            transform.position = Vector2.Lerp(originalPosition, endPosition, sinInterpolation);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the door has exactly reached the end position
        transform.position = endPosition;

        if (stayOpenUntilCommand)
        {
            yield return new WaitUntil(() => _canClose);
            _canClose = false;
        }
        else
        {
            // Door stays open for doorStayOpenTime seconds
            yield return new WaitForSeconds(doorStayOpenTime);
        }

        // Start shaking the door before it closes
        yield return StartCoroutine(ShakeEffect());

        // Close the door
        elapsedTime = 0;
        while (elapsedTime < openTime)
        {
            float sinInterpolation = 0.5f * (1 - Mathf.Cos(Mathf.PI * elapsedTime / openTime));
            transform.position = Vector2.Lerp(endPosition, originalPosition, sinInterpolation);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the door has exactly reached the original position
        transform.position = originalPosition;

        isDoorMoving = false;
    }
}
