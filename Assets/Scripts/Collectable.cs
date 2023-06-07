using UnityEngine;

public class Collectable : MonoBehaviour
{
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private float floatSpeed = 4.0f;
    [SerializeField] private float floatAmount = 0.25f;

    private Vector3 originalPosition;
    private float offset;

    private void Start()
    {
        originalPosition = transform.position;
        offset = floatAmount; // Offset equals to float amount to prevent going below original position
    }

    private void Update()
    {
        Float();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(playerTag))
        {
            Debug.Log("Collectable collected!");
            gameObject.SetActive(false);
        }
    }

    private void Float()
    {
        float newY = originalPosition.y + offset + Mathf.Sin(Time.time * floatSpeed) * floatAmount;
        transform.position = new Vector3(originalPosition.x, newY, originalPosition.z);
    }
}