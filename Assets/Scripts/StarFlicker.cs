using UnityEngine;

using System.Collections;

public class StarFlicker : MonoBehaviour
{
    private UnityEngine.Rendering.Universal.Light2D myLight;
    private float originalIntensity;
    private float originalRadius;

    [SerializeField] private float minFlickerTime = 0.5f;
    [SerializeField] private float maxFlickerTime = 2f;
    [SerializeField] private float maxRadiusPercentageIncrease = 0.2f; // 20% increase
    [SerializeField] private float maxIntensityPercentageIncrease = 0.2f; // 20% increase

    private float currentFlickerTime;

    private void Awake()
    {
        myLight = GetComponent<UnityEngine.Rendering.Universal.Light2D>();
        originalIntensity = myLight.intensity;
        originalRadius = myLight.pointLightOuterRadius;
    }

    private void Start()
    {
        StartCoroutine(FlickerLight());
    }

    private IEnumerator FlickerLight()
    {
        while(true)
        {
            // Calculate new random values for radius and intensity
            float newOuterRadius = originalRadius * (1f + Random.Range(0f, maxRadiusPercentageIncrease));
            float newIntensity = originalIntensity * (1f + Random.Range(0f, maxIntensityPercentageIncrease));

            // Update the flicker time for the next cycle
            currentFlickerTime = Random.Range(minFlickerTime, maxFlickerTime);

            float elapsedTime = 0f;

            // Smoothly interpolate from the original values to the new random values
            while(elapsedTime < currentFlickerTime)
            {
                myLight.pointLightOuterRadius = Mathf.Lerp(originalRadius, newOuterRadius, (elapsedTime / currentFlickerTime));
                myLight.intensity = Mathf.Lerp(originalIntensity, newIntensity, (elapsedTime / currentFlickerTime));
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // Reset light values to their original states
            myLight.pointLightOuterRadius = originalRadius;
            myLight.intensity = originalIntensity;

            yield return new WaitForSeconds(currentFlickerTime);
        }
    }
}
