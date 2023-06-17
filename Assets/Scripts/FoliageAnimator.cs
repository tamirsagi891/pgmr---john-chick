using UnityEngine;

public class FoliageAnimator : MonoBehaviour
{
    [SerializeField] private float maxOffset = 0.05f; // Maximum offset for position change
    [SerializeField] private float maxRotation = 2f; // Maximum degrees for rotation change
    [SerializeField] private float noiseScale = 0.2f; // How fast noise changes over time
    [SerializeField] private float changeInterval = 1f; // Time interval to change the movement

    private Vector3 originalPosition;
    private Quaternion originalRotation;

    private float timeSinceLastChange;

    void Start()
    {
        originalPosition = transform.position;
        originalRotation = transform.rotation;
        timeSinceLastChange = changeInterval; // So that movement change happens immediately on start
    }

    void Update()
    {
        timeSinceLastChange += Time.deltaTime;

        if (timeSinceLastChange >= changeInterval)
        {
            timeSinceLastChange -= changeInterval;

            // Generate perlin noise for position and rotation
            float offsetX = Mathf.PerlinNoise(Time.time * noiseScale, 0) * 2 - 1; // Generate value between -1 and 1
            float offsetY = Mathf.PerlinNoise(0, Time.time * noiseScale) * 2 - 1; // Generate value between -1 and 1
            float rotationZ = Mathf.PerlinNoise(Time.time * noiseScale, Time.time * noiseScale) * 2 - 1; // Generate value between -1 and 1

            // Calculate new position and rotation
            Vector3 newPosition = originalPosition + new Vector3(maxOffset * offsetX, maxOffset * offsetY, 0);
            Quaternion newRotation = originalRotation * Quaternion.Euler(0, 0, maxRotation * rotationZ);

            // Apply new position and rotation
            transform.position = newPosition;
            transform.rotation = newRotation;
        }
    }
}