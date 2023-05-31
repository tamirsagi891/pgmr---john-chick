using UnityEngine;

[ExecuteInEditMode]
public class ParticleScaler : MonoBehaviour
{
    private Vector2 originalObjectSize;
    private Vector3 originalParticleScale;
    private ParticleSystem[] particleSystems;

    private void Start()
    {
        if (!GetComponent<SpriteRenderer>())
            return;
            
        originalObjectSize = GetComponent<SpriteRenderer>().bounds.size;
        particleSystems = GetComponentsInChildren<ParticleSystem>();
        
        if (particleSystems.Length > 0)
            originalParticleScale = particleSystems[0].transform.localScale;
    }

    private void Update()
    {
        ScaleParticles();
    }

    private void ScaleParticles()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (!spriteRenderer)
            return;
            
        // Get the current size of the GameObject
        Vector2 currentObjectSize = spriteRenderer.bounds.size;

        // Calculate the scale factor based on the GameObject size
        float scaleFactor = Mathf.Max(currentObjectSize.x, currentObjectSize.y) / Mathf.Max(originalObjectSize.x, originalObjectSize.y);

        foreach (ParticleSystem particles in particleSystems)
        {
            // Apply scale to each Particle System
            ParticleSystem.MainModule mainModule = particles.main;
            mainModule.scalingMode = ParticleSystemScalingMode.Hierarchy;
            particles.transform.localScale = originalParticleScale * scaleFactor;
        }
    }
}